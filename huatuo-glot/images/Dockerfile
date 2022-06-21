ARG GLOT_IMAGE
FROM ${GLOT_IMAGE}

ADD code-runner_linux-x64.tar.gz /

RUN getent group glot || groupadd glot
RUN id -u somename &>/dev/null || useradd -d /home/glot -g glot -s /bin/bash glot

WORKDIR /home/glot

USER root
RUN chmod -R 777 /home/glot
USER glot

ENTRYPOINT []
CMD [ "/code-runner", "--path", "/home/glot" ]