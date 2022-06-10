# glot-docker-huatuo
## [演示站点](http://javap.zicp.net/new/csharp)
![image](https://user-images.githubusercontent.com/49626119/172984992-ba0c300a-cf58-4207-83df-8833fc9d4482.png)

## 部署脚本
```
# 在能够翻墙的环境下运行
chmod +x glot-www/glot-www/*.sh

docker run --volume $(pwd)/glot-www/glot-www:/build --rm prasmussen/glot-www-build:latest

docker-compose build
docker-compose up -d pg
sleep 10s
docker-compose up -d
```

## 构建glot/huatuo镜像
### 可以选择在docker下打包，参考[unity-build-in-docker](unity-build-in-docker/docker-compose.yml)
证书文件名Unity_lic.ulf [证书获取流程](https://game.ci/docs/gitlab/activation)
```
# 挂在存放unity证书的目录，推荐使用nas存储，这样可以多台机器使用同个证书
volumes:
  unity:
    driver_opts:
      type: "nfs"
      o: "addr=nas.mgame.cn,nolock,soft,rw"
      device: ":/volume1/unity"
```

1. 拷贝[TestDriver](TestDriver.cs) 和 [CommandLine](CommandLine.dll)到unity工程
2. 打包
``` c#
# 构建前自行制作Linux版本unity可执行程序，采用Server Build打包

# 打包脚本样例
        [MenuItem("HuaTuo/BuildForGlot")]
        private static void BuildForGlot()
        {
            var buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.options = BuildOptions.EnableHeadlessMode;
            buildPlayerOptions.targetGroup = BuildTargetGroup.Standalone;

            buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
            buildPlayerOptions.locationPathName = "Glot-Windows/huatuo.exe";

#if UNITY_EDITOR_LINUX
            buildPlayerOptions.target = BuildTarget.StandaloneLinux64;
            buildPlayerOptions.locationPathName = "Glot-Linux/huatuo";
#endif
            var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
            if (buildReport.summary.result == BuildResult.Succeeded)
            {
                Debug.Log("构建成功");
            }
            else
            {
                Debug.LogError("构建失败");
                if (Application.isBatchMode) Application.Quit(1);
            }
        }
```
3. 将Glot-Linux目录拷贝到当前目录
4. 构建镜像
```
# glot/huatuo 镜像作为c#代码运行的容器
# 构建命令
docker build -f Dockerfile.glot -t glot/huatuo .
```
