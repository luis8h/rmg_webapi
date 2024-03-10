#!/bin/bash

docker login 192.168.188.179:5000
docker buildx build -t 192.168.188.179:5000/luis8h/rmg_webapi:sta ./
docker push 192.168.188.179:5000/luis8h/rmg_webapi:sta

