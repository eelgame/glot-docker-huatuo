FROM ubuntu:20.04

#ADD docker-run_linux-x64.tar.gz /

RUN apt-get update && apt-get install -y cargo
ADD docker-run /docker-run

WORKDIR /docker-run
RUN cargo build
ENTRYPOINT [ "cargo", "run" ]