ARG GLOT_IMAGE
FROM ${GLOT_IMAGE}

ADD code-runner_linux-x64.tar.gz /

RUN groupadd glot
RUN useradd -d /home/glot -g glot -s /bin/bash glot

WORKDIR /home/glot

RUN chmod -R 777 /home/glot
ENTRYPOINT []
CMD [ "/code-runner", "--path", "/home/glot" ]