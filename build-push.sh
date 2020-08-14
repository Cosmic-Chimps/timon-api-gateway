#!/usr/bin/env bash

docker build . -t jjchiw/timon-api-gateway:$(git rev-parse --short HEAD)
docker push jjchiw/timon-api-gateway:$(git rev-parse --short HEAD)
git rev-parse --short HEAD