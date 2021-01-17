robocopy 'C:\dev\orleans\Artifacts\Debug\' 'packages' /MIR
docker build . -t reuben.azurecr.io/dictionary-app &&
docker push reuben.azurecr.io/dictionary-app &&
kubectl apply -f ./deployment.yaml &&
kubectl rollout restart deployment/dictionary-app
