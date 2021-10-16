# STAN Integration Tests

## Reference

- [NATS Documentation](https://docs.nats.io/)
- [nsc Documentation](https://nats-io.github.io/nsc/)
- [NATS.Client Documetation](https://github.com/nats-io/nats.net)

## Prerequisites

- Have a STAN cluster running **with TLS and Authentication enabled**. You can achieve this by
  using [Macula Jetstream Repository](https://git.macula.io/logatron-io/deployment/infrastructure/nats-jetstream) in
  the **imperative** folder. A more streamlined GitOps solution will follow soon.
- add the following to **~/.<your-fav-sh>rc**

```bash
alias nb="docker run --rm -it -v "${PWD}"/nsc:/nsc synadia/nats-box:latest"
alias nb-k8s="kubectl exec -i -t pod/nats-box -- sh"
```

- **!! DO NOT FORGET !!**

```bash
$ source ~/.<your-fav-sh>rc
```

## Defining a user on the NATS Cluster

- start natsbox on the cluster

```bash
$ nb-k8s
```

- define an operator

```bash
# nsc add operator 
```

## Test Setup

## Reference

- [nsc Documentation](https://nats-io.github.io/nsc/)
  


