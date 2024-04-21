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
