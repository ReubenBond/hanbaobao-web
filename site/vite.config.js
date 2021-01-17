import vue from '@vitejs/plugin-vue'

export default {
    plugins: [vue()],
    server: {
        proxy: {
            '/api': {
                target: 'http://localhost:5000',
                changeOrigin: true
            }
        }
    },
    build: {
        outDir: '../HanBaoBaoWeb/wwwroot'
    }
}