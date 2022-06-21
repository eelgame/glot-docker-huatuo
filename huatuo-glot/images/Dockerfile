ARG GLOT_IMAGE
FROM ${GLOT_IMAGE}

ADD code-runner_linux-x64.tar.gz /

USER root
RUN mkdir -p /home/glot && chmod -R 777 /home/glot
USER glot

ENTRYPOINT []
CMD [ "/code-runner", "--path", "/home/glot" ]