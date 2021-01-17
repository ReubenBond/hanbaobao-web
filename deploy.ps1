pushd HanBaoBaoWeb/site
vite build --config ./vite.config.js --emptyOutDir &&
popd
docker build . -t reuben.azurecr.io/hanbaobao &&
docker push reuben.azurecr.io/hanbaobao &&
kubectl apply -f ./deployment.yaml &&
kubectl rollout restart deployment/hanbaobao
