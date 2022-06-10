# glot-docker-huatuo

## 部署脚本
```
chmod +x glot-www/glot-www/*.sh

docker run --volume $(pwd)/glot-www/glot-www:/build --rm prasmussen/glot-www-build:latest

docker-compose build
docker-compose up -d pg
sleep 10s
docker-compose up -d
```
