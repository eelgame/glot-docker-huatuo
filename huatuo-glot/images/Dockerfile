ARG GLOT_IMAGE
FROM ${GLOT_IMAGE}

ADD code-runner_linux-x64.tar.gz /

USER root
RUN mkdir -p /home/glot && chmod -R 777 /home/glot
RUN rm -rf /home/glot/runner
RUN cp /code-runner /home/glot/runner
RUN chmod +x /home/glot/runner