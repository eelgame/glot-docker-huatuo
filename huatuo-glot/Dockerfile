FROM unityci/editor:ubuntu-2020.3.33f1-linux-il2cpp-1.0.1
ENV TZ=Asia/Shanghai

RUN sed -i 's/archive.ubuntu.com/mirrors.aliyun.com/g' /etc/apt/sources.list

RUN apt-get update && DEBIAN_FRONTEND=noninteractive apt-get install tzdata && apt-get install -y \
  git \
  subversion \
  python3 python3-pip \
  aapt zipalign adb \
  wget unzip openjdk-8-jdk dos2unix\
  && apt clean \
  && rm -rf /var/lib/apt/lists/* 
    
ENV JAVA_HOME /usr/lib/jvm/java-8-openjdk-amd64/
RUN export JAVA_HOME
   
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone
RUN dpkg-reconfigure --frontend noninteractive tzdata
RUN update-alternatives --install /usr/bin/python python /usr/bin/python3 1

ADD entrypoint.sh /root/entrypoint.sh
RUN dos2unix /root/entrypoint.sh
RUN chmod +x /root/entrypoint.sh
CMD []  
ENTRYPOINT ["/root/entrypoint.sh"]