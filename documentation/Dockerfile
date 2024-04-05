FROM python:3-alpine

ARG USER=1001

RUN adduser -h /usr/src/mkdocs -D -u $USER mkdocs \
&& apk add bash \
&& apk add git 

ENV PATH="${PATH}:/usr/src/mkdocs/.local/bin"

USER mkdocs
RUN mkdir -p /usr/src/mkdocs/build
WORKDIR /usr/src/mkdocs/build

RUN pip install --upgrade pip
COPY requirements.txt .
RUN \
    --mount=type=cache,target=/root/.cache \
    pip install -r requirements.txt


ENTRYPOINT ["/usr/src/mkdocs/.local/bin/mkdocs"]
