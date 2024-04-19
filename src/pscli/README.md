# pscli

# With Docker

# Create image (from ps-viewer root)

```console
docker build --tag pscli-img -f src/pscli/Dockerfile .
```

# Create container

```console
docker run -it --rm -v ${PWD}/src/pscli/temp:/App/temp pscli-img <pscli arguments>
```

# Docker with localhost MongoDB

## Pull the MongoDB Docker Image

```console
docker pull mongodb/mongodb-community-server:4.4.18-ubuntu2004
```

## Run the Image as a Container

```console
docker run --name mongodb -p 27017:27017 -d mongodb/mongodb-community-server:4.4.18-ubuntu2004
```

docker volume create mongodbdata
docker run --name mongodb -p 27017:27017 -v mongodbdata:/data/db mongodb/mongodb-community-server:4.4.18-ubuntu2004
