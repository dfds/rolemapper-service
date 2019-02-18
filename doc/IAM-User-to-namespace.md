# IAM Role to namespace chain

Users can gain access to a Kubernetes namespace based on the capabilities they work on.
This is facilitated by giving every capability a AWS IAM role that can be used to log into Kubernetes with.
The Kubernetes service is responsible for creating the chain that links a IAM role to a namespace.

The chain that links a IAM role to a namespace can be describe as following:

* A IAM role which is a AWS specific concept is mapped to a Kubernetes User by the aws-iam-authenticator process.
* The User will be a part of a group named after the capability.
* A namespace named after the capability will be created.
* A all access role is created for the namespace.
* The all access role is bound to the same group the user is in closing the final link between the IAM Role and the Namespace.

Please see IAM-User-to-namespace.mmd for flowchart of the chain

## Sources

http://marcinkaszynski.com/2018/07/12/eks-auth.html
https://kubernetes.io/docs/reference/access-authn-authz/rbac/
https://github.com/kubernetes-sigs/aws-iam-authenticator
https://github.com/Versent/saml2aws