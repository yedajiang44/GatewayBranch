#!/bin/bash
dotnet publish ./src/GatewayBranch.Application/GatewayBranch.Application.csproj -c Release -o .output
docker build --pull --rm --no-cache -f "Dockerfile" -t yedajiang44/gatewaybranch "."

while true; do
    stty -icanon min 0 time 100
    echo -n "是否推送镜像(yes or no)?"
    read -r Arg
    case $Arg in
    Y | y | YES | yes)
        break
        ;;
    N | n | NO | no)
        exit
        ;;
    "") #Autocontinue
        break ;;
    esac
done

while true; do
    stty -icanon min 0 time 100
    echo -n 输入镜像tag：
    read -r tag
    case $tag in
    "") ;;
    *)
        break
        ;;

    esac
done

echo '准备镜像...'
docker tag yedajiang44/gatewaybranch yedajiang44/gatewaybranch:"$tag"
docker tag yedajiang44/gatewaybranch registry.cn-hangzhou.aliyuncs.com/yedajiang44/gatewaybranch
docker tag yedajiang44/gatewaybranch registry.cn-hangzhou.aliyuncs.com/yedajiang44/gatewaybranch:"$tag"
echo '准备完毕...'

echo '准备推送镜像...'
echo '推送至docker hub...'
docker push yedajiang44/gatewaybranch:"$tag"
docker push yedajiang44/gatewaybranch
echo '推送镜像完毕...'

echo '推送至阿里云...'
docker push registry.cn-hangzhou.aliyuncs.com/yedajiang44/gatewaybranch:"$tag"
docker push registry.cn-hangzhou.aliyuncs.com/yedajiang44/gatewaybranch
echo '推送镜像完毕...'
