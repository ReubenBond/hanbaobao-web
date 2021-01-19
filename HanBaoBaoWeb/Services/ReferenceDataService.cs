using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Data.Common;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;

namespace DictionaryApp
{
    /// <summary>
    /// Even still, a separate service such as Solr or ElasticSearch would be used to perform full-text-search.
    /// For this simple dictionary application, this simple approach is appropriate.
    /// </remarks>
    public class ReferenceDataService
    {
        private readonly string _connectionString;
        private readonly TaskScheduler _scheduler;
        private const string Ordering = "hsk_level not null desc, hsk_level asc, part_of_speech not null desc, frequency desc";

        public ReferenceDataService()
        {
            var builder = new SqliteConnectionStringBuilder("Data Source=Data/hanbaobao.db;")
            {
                Mode = SqliteOpenMode.ReadOnly,
                Cache = SqliteCacheMode.Shared,
            };

            _connectionString = builder.ToString();
            _scheduler = new ConcurrentExclusiveSchedulerPair(TaskScheduler.Default, maxConcurrencyLevel: 1).ExclusiveScheduler;
        }

        public Task<List<string>> QueryHeadwordsByAnyAsync(string query)
        {
            // Why are we scheduling the database calls on a specific TaskScheduler?
            // The answer is centered around SQLite being a synchronous library and performs blocking IO operations.
            // There are asynchronous overloads, but they call the synchronous methods internally.
            // By running many blocking IO operations on the .NET Thread Pool in parallel, we can starve the pool and cause severe performance
            // degradation, which can result in a denial of service.
            // A better approach for handling a synchronous library like this would be to have a dedicated pool of threads, rather than borrowing
            // ThreadPool threads like this implementation does.
            return Task.Factory.StartNew(() =>
            {
                if (IsProbablyCjk(query))
                {
                    return QueryHeadwordByHeadword(query);
                }

                return QueryHeadwordByDefinition(query);
            },
            CancellationToken.None,
            TaskCreationOptions.RunContinuationsAsynchronously,
            _scheduler);  
        }

        public Task<List<TermDefinition>> QueryByAnyAsync(string query)
        {
            if (IsProbablyCjk(query))
            {
                return QueryByHeadwordAsync(query);
            }

            return Task.Factory.StartNew(() =>
            {
                return QueryByDefinition(query);
            },
            CancellationToken.None,
            TaskCreationOptions.RunContinuationsAsynchronously,
            _scheduler);  
        }

        public Task<List<TermDefinition>> QueryByHeadwordAsync(string query)
        {
            return Task.Factory.StartNew(() =>
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();
                var cmd = new SqliteCommand("select * from dictionary where simplified=$term or traditional=$term order by " + Ordering, connection);
                cmd.Parameters.AddWithValue("$term", query);
                cmd.Prepare();

                var reader = cmd.ExecuteReader();
                return ReadAllAsTermDefinition(reader);
            },
            CancellationToken.None,
            TaskCreationOptions.RunContinuationsAsynchronously,
            _scheduler);  
        }

        private List<string> QueryHeadwordByHeadword(string query)
        {
            using var connection = new SqliteConnection(_connectionString);
            using var cmd = new SqliteCommand("select distinct simplified from dictionary where simplified=$term or traditional=$term order by " + Ordering, connection);
            connection.Open();
            cmd.Parameters.AddWithValue("$term", query);
            var reader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            return ReadHeadwords(reader);
        }

        private List<string> QueryHeadwordByDefinition(string query)
        {
            using var connection = new SqliteConnection(_connectionString);
            using var cmd = new SqliteCommand("select distinct simplified from dictionary where rowid in (select rowid from fts_definition where fts_definition match $query) order by " + Ordering, connection);
            connection.Open();
            cmd.Parameters.AddWithValue("$query", query);

            var reader = cmd.ExecuteReader();
            return ReadHeadwords(reader);
        }

        private List<TermDefinition> QueryByDefinition(string query)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var cmd = new SqliteCommand("select * from dictionary where rowid in (select rowid from fts_definition where fts_definition match $query) order by " + Ordering, connection);
            cmd.Parameters.AddWithValue("$query", query);

            var reader = cmd.ExecuteReader();
            return ReadAllAsTermDefinition(reader);
        }

        private static bool IsProbablyCjk(string text)
        {
            foreach (var c in text)
            {
                var val = (int)c;
                if (IsInRange(UnicodeRanges.CjkUnifiedIdeographs, c) || IsInRange(UnicodeRanges.CjkUnifiedIdeographsExtensionA, c))
                {
                    return true;
                }
            }

            return false;

            static bool IsInRange(UnicodeRange range, char c)
            {
                var val = (int)c;
                return val >= range.FirstCodePoint && val <= (range.FirstCodePoint + range.Length);
            }
        }

        private static List<string> ReadHeadwords(DbDataReader reader)
        {
            int simplifiedColId = reader.GetOrdinal("simplified");

            var results = new List<string>();
            while (reader.Read())
            {
                if (!reader.IsDBNull(simplifiedColId))
                {
                    results.Add(reader.GetString(simplifiedColId));
                }
            }

            return results;
        }

        private static List<TermDefinition> ReadAllAsTermDefinition(DbDataReader reader)
        {
            int rowIdColId = reader.GetOrdinal("rowid");
            int simplifiedColId = reader.GetOrdinal("simplified");
            int traditionalColId = reader.GetOrdinal("traditional");
            int pinyinColId = reader.GetOrdinal("pinyin");
            int definitionColId = reader.GetOrdinal("definition");
            int hskLevelColId = reader.GetOrdinal("hsk_level");
            int classifierColId = reader.GetOrdinal("classifier");
            int posColId = reader.GetOrdinal("part_of_speech");
            int frequencyColId = reader.GetOrdinal("frequency");
            int conceptColId = reader.GetOrdinal("concept");
            int topicColId = reader.GetOrdinal("topic");
            int parentTopicColId = reader.GetOrdinal("parent_topic");
            int notesColId = reader.GetOrdinal("notes");

            var results = new List<TermDefinition>();
            while (reader.Read())
            {
                var result = new TermDefinition();
                result.Id = reader.GetInt64(rowIdColId);
                result.Simplified = reader.IsDBNull(simplifiedColId) ? null : reader.GetString(simplifiedColId);
                result.Traditional = reader.IsDBNull(traditionalColId) ? null : reader.GetString(traditionalColId);
                result.Pinyin = reader.IsDBNull(pinyinColId) ? null : reader.GetString(pinyinColId);
                result.Definition = reader.IsDBNull(definitionColId) ? null : reader.GetString(definitionColId);
                result.Classifier = reader.IsDBNull(classifierColId) ? null : reader.GetString(classifierColId);
                result.HskLevel = reader.IsDBNull(hskLevelColId) ? 0 : reader.GetInt32(hskLevelColId);
                result.PartOfSpeech = reader.IsDBNull(posColId) ? null : PartOfSpeechToStrings(reader.GetInt64(posColId));
                result.Frequency = reader.IsDBNull(frequencyColId) ? 0 : reader.GetDouble(frequencyColId);
                result.Concept = reader.IsDBNull(conceptColId) ? null : reader.GetString(conceptColId);
                result.Topic = reader.IsDBNull(topicColId) ? null : reader.GetString(topicColId);
                result.ParentTopic = reader.IsDBNull(parentTopicColId) ? null : reader.GetString(parentTopicColId);
                result.Notes = reader.IsDBNull(notesColId) ? null : reader.GetString(notesColId);
                results.Add(result);
            }

            return results;
        }

        public static List<string> PartOfSpeechToStrings(long input)
        {
            List<string> result = new List<string>();
            if ((input & ((long)1)) != 0)
            {
                result.Add("ADDRESS");
            }

            if ((input & (((long)1 << 1))) != 0)
            {
                result.Add("ADJECTIVE");
            }

            if ((input & ((long)1 << 2)) != 0)
            {
                result.Add("ADVERB");
            }

            if ((input & ((long)1 << 3)) != 0)
            {
                result.Add("AUXILIARY VERB");
            }

            if ((input & ((long)1 << 4)) != 0)
            {
                result.Add("BOUND MORPHEME");
            }

            if ((input & ((long)1 << 5)) != 0)
            {
                result.Add("SET PHRASE");
            }

            if ((input & ((long)1 << 6)) != 0)
            {
                result.Add("CITY");
            }

            if ((input & ((long)1 << 7)) != 0)
            {
                result.Add("COMPLEMENT");
            }

            if ((input & ((long)1 << 8)) != 0)
            {
                result.Add("CONJUNCTION");
            }

            if ((input & ((long)1 << 9)) != 0)
            {
                result.Add("COUNTRY");
            }

            if ((input & ((long)1 << 10)) != 0)
            {
                result.Add("DATE");
            }

            if ((input & ((long)1 << 11)) != 0)
            {
                result.Add("DETERMINER");
            }

            if ((input & ((long)1 << 12)) != 0)
            {
                result.Add("DIRECTIONAL");
            }

            if ((input & ((long)1 << 13)) != 0)
            {
                result.Add("EXPRESSION");
            }

            if ((input & ((long)1 << 14)) != 0)
            {
                result.Add("FOREIGN TERM");
            }

            if ((input & ((long)1 << 15)) != 0)
            {
                result.Add("GEOGRAPHY");
            }

            if ((input & ((long)1 << 16)) != 0)
            {
                result.Add("IDIOM");
            }

            if ((input & ((long)1 << 17)) != 0)
            {
                result.Add("INTERJECTION");
            }

            if ((input & ((long)1 << 18)) != 0)
            {
                result.Add("MEASURE WORD");
            }

            if ((input & ((long)1 << 19)) != 0)
            {
                result.Add("MEASUREMENT");
            }
            //if ((input & ((long)1 << 20)) != 0) result.Add("NAME");
            if ((input & ((long)1 << 20)) != 0 || (input & ((long)1 << 21)) != 0)
            {
                result.Add("NOUN");
            }

            if ((input & ((long)1 << 22)) != 0)
            {
                result.Add("NUMBER");
            }

            if ((input & ((long)1 << 23)) != 0)
            {
                result.Add("NUMERAL");
            }

            if ((input & ((long)1 << 24)) != 0)
            {
                result.Add("ONOMATOPOEIA");
            }

            if ((input & ((long)1 << 25)) != 0)
            {
                result.Add("ORDINAL");
            }

            if ((input & ((long)1 << 26)) != 0)
            {
                result.Add("ORGANIZATION");
            }

            if ((input & ((long)1 << 27)) != 0)
            {
                result.Add("PARTICLE");
            }

            if ((input & ((long)1 << 28)) != 0)
            {
                result.Add("PERSON");
            }

            if ((input & ((long)1 << 29)) != 0)
            {
                result.Add("PHONETIC");
            }

            if ((input & ((long)1 << 30)) != 0)
            {
                result.Add("PHRASE");
            }

            if ((input & ((long)1 << 31)) != 0)
            {
                result.Add("PLACE");
            }

            if ((input & ((long)1 << 32)) != 0)
            {
                result.Add("PREFIX");
            }

            if ((input & ((long)1 << 33)) != 0)
            {
                result.Add("PREPOSITION");
            }

            if ((input & ((long)1 << 34)) != 0)
            {
                result.Add("PRONOUN");
            }

            if ((input & ((long)1 << 35)) != 0)
            {
                result.Add("PROPER NOUN");
            }

            if ((input & ((long)1 << 36)) != 0)
            {
                result.Add("QUANTITY");
            }

            if ((input & ((long)1 << 37)) != 0)
            {
                result.Add("RADICAL");
            }

            if ((input & ((long)1 << 38)) != 0)
            {
                result.Add("SUFFIX");
            }

            if ((input & ((long)1 << 39)) != 0)
            {
                result.Add("TEMPORAL");
            }

            if ((input & ((long)1 << 40)) != 0)
            {
                result.Add("TIME");
            }

            if ((input & ((long)1 << 41)) != 0)
            {
                result.Add("VERB");
            }

            return result;
        }
    }
}
