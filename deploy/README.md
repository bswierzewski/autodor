# Mikr.us + Dokploy + Cloudflare Deployment Guide

This document describes how to deploy a new production instance from scratch on Mikr.us using Dokploy, Cloudflare, PostgreSQL, S3 backups, and Clerk.

The guide uses `bswierzewski.fun` as the example domain and replaces all other sensitive values with placeholders.

## 1. Target Architecture

At the end of this process you will have:

- a Mikr.us server reachable over SSH,
- a domain delegated to Cloudflare,
- Cloudflare DNS pointing the domain to the Mikr.us IPv6 address,
- Dokploy installed on the server,
- the Dokploy panel published at `dokploy.bswierzewski.fun`,
- a PostgreSQL database managed by Dokploy,
- an application deployed from a private GHCR image,
- production environment variables configured in Dokploy,
- S3 storage configured for file storage and backups,
- automated database backups,
- Clerk configured for production with the required DNS records.

## 2. Prerequisites

Prepare the following before you start:

- a registered domain, for example `bswierzewski.fun`,
- a Mikr.us server with SSH access,
- the server IPv6 address from Mikr.us,
- a local SSH client such as PuTTY,
- a GitHub personal access token with permission to pull private GHCR images,
- a built container image published to GHCR, for example `ghcr.io/<github-namespace>/<project-name>:latest`,
- S3-compatible object storage credentials for backups and application files,
- a production Clerk instance,
- the production values required by this repository's `.env.example`.

## 3. Prepare the Mikr.us Server

Connect to the server over SSH and perform the basic operating system setup.

Notes:

- Dokploy uses port `3000` for its initial local admin service during installation, but on Mikr.us this port is not exposed publicly and should be treated as SSH-tunnel-only access.
- The application in this repository listens on container port `8080`, but that port does not need to be exposed publicly because Traefik in Dokploy will route traffic to it internally.
- Dokploy's official recommendation is at least 2 GB RAM and 30 GB disk. If the Mikr.us plan is smaller, expect tighter build and storage limits.

## 4. Add the Domain to Cloudflare

Create a Cloudflare account if needed and add the domain `bswierzewski.fun` to Cloudflare.

Cloudflare will provide two authoritative name servers. Go to your domain registrar panel and replace the current name servers with the ones provided by Cloudflare.

This step is required so that:

- Cloudflare becomes the authoritative DNS provider for the domain,
- the domain can resolve the Mikr.us server IPv6 address through Cloudflare,
- Cloudflare-managed records can later be used for Dokploy and Clerk.

Wait until the domain status in Cloudflare changes to active.

## 5. Configure Cloudflare DNS for the Server

After the domain is active in Cloudflare, create the production AAAA records that point to the Mikr.us IPv6 address.

Example DNS records:

```dns
;; AAAA Records
*.bswierzewski.fun.    1    IN    AAAA    <mikrus-ipv6> ; proxied
bswierzewski.fun.      1    IN    AAAA    <mikrus-ipv6> ; proxied
```

Cloudflare recommendations:

- keep the root record for `bswierzewski.fun`,
- keep the wildcard record for `*.bswierzewski.fun`,
- enable proxying for the application and panel traffic,
- do not proxy Clerk CNAME records later in this guide.

Validation checklist:

- `bswierzewski.fun` resolves to the Mikr.us IPv6,
- `dokploy.bswierzewski.fun` will also resolve because of the wildcard record,
- the domain registrar now points to Cloudflare name servers.

## 6. Install Dokploy

Install Dokploy directly on the Mikr.us server.

```bash
curl -sSL https://dokploy.com/install.sh | sh
```

What this does:

- installs Docker if it is missing,
- initializes Docker Swarm,
- installs Dokploy and Traefik,
- starts the initial Dokploy admin UI on the server-side port `3000`.

Initial access on Mikr.us:

- do not expect direct access to `http://<server-ip>:3000`,
- use an SSH tunnel as described in the next section,
- open Dokploy locally through the tunnel at `http://127.0.0.1:13000`.

During first login:

- create the Dokploy admin account,
- finish the bootstrap flow,
- keep using the SSH tunnel until the Dokploy panel is available on its production domain.

## 7. Create a PuTTY SSH Tunnel to Dokploy

On Mikr.us, the Dokploy panel must be accessed through an SSH tunnel during the initial setup because port `3000` is not publicly exposed.

Goal:

- local address: `127.0.0.1:13000`,
- remote target: `127.0.0.1:3000` on the Mikr.us server.

PuTTY configuration:

1. Open PuTTY.
2. In `Session`, enter the Mikr.us host and SSH port.
3. Go to `Connection` -> `SSH` -> `Tunnels`.
4. In `Source port`, enter `13000`.
5. In `Destination`, enter `127.0.0.1:3000`.
6. Choose `Local`.
7. Leave `Auto` selected.
8. Click `Add`.
9. Go back to `Session` and save the profile.
10. Connect over SSH.

After the SSH session is open, access Dokploy locally in the browser at:

```text
http://127.0.0.1:13000
```

This tunnel is only needed until the Dokploy panel is published on a real domain.

## 8. Publish the Dokploy Panel on a Domain

Once the initial Dokploy setup is complete, configure a proper panel domain so the UI is reachable without an SSH tunnel.

Recommended hostname:

```text
dokploy.bswierzewski.fun
```

In Dokploy:

1. Open the Web Server settings.
2. Add the panel domain `dokploy.bswierzewski.fun`.
3. Enable HTTPS.
4. Use Let's Encrypt unless you have a custom certificate strategy.
5. Save the configuration.
6. Verify that the panel opens at `https://dokploy.bswierzewski.fun`.

After the panel domain works correctly:

- you can stop using the PuTTY tunnel,
- you can use this domain for webhooks and CI/CD callbacks.

## 9. Recommended Cloudflare Settings

Before publishing the application, configure the main Cloudflare security and TLS settings.

Recommended values:

- SSL/TLS mode: `Full (strict)` once Let's Encrypt is active in Dokploy,
- Always Use HTTPS: enabled,
- Automatic HTTPS Rewrites: enabled,
- HTTP to HTTPS redirect: handled either in Cloudflare or in Dokploy, but avoid duplicate redirect logic if you see loops,
- Proxy status: enabled for the app and Dokploy panel domains.

Do not proxy Clerk verification CNAME records. Those must remain DNS-only.

## 10. Create the Project in Dokploy

Create a new project in Dokploy for the production environment.

Recommended structure:

- Project: `<project-name>`
- Environment: `production`

Inside the project you will add:

- one PostgreSQL database,
- one migration job or one-off service,
- one application service,
- optional shared environment variables,
- domains,
- backup configuration.

## 11. Add the Image Registry Credentials

Before creating the application service, configure the private container registry in Dokploy.

For GHCR use values similar to:

- Registry name: `GHCR`
- Registry URL: `ghcr.io`
- Username: `<github-login>`
- Password: `<github-personal-access-token>`

Notes:

- the token must be allowed to read packages,
- if the image is private and Dokploy cannot authenticate, the deployment will fail before the container starts,
- keep the image tag stable for production, for example `latest` or a version tag.

## 12. Add the PostgreSQL Database

Create a PostgreSQL database service inside the Dokploy project.

Recommended settings:

- use a persistent volume,
- use a strong generated password,
- keep the database internal-only,
- do not expose PostgreSQL publicly unless you have a specific operational reason.

Save the connection details because the application will need them in its environment variables.

## 13. Add the Application Service

Create an Application service that deploys the prebuilt GHCR image.

Example image configuration:

- Docker Image: `ghcr.io/<github-namespace>/autodor-app:latest`
- Registry: the GHCR registry created earlier
- Exposed container port in domain configuration: `8080`

Important repository-specific requirement:

- this repository's final container listens on port `8080`,
- the frontend is already embedded into the API image,
- the frontend build needs `VITE_CLERK_PUBLISHABLE_KEY` as a build-time value if you build from source in Dokploy,
- if you deploy a prebuilt image, the value must already be present during image build in CI.

Critical Mikr.us compatibility setting:

In the application service open:

`Advanced` -> `Cluster Settings` -> `Swarm Settings` -> `Endpoint Spec`

Set the endpoint mode to:

```text
DNS Round Robin
```

This is required on Mikr.us-style LXC environments where the default Swarm VIP networking can cause internal service resolution problems.

## 13.1 Add the Migration Job

This repository now publishes a dedicated migrator image alongside the API image.

Recommended image configuration:

- Docker Image: `ghcr.io/<github-namespace>/autodor-migrator:latest`
- Registry: the GHCR registry created earlier
- Do not expose any public port

Recommended behavior:

- run the migrator as a one-off job before each API deployment,
- use the same `ConnectionStrings__Default` value as the API,
- fail the rollout if the migrator exits with a non-zero code,
- do not keep the migrator as a long-running public service.

Required Dokploy settings for the migrator:

- `Advanced` -> `Swarm Settings` -> `Restart Policy` -> `Condition: none`
- `Advanced` -> `Mode` -> `ReplicatedJob`
- `Advanced` -> `Mode` -> `TotalCompletions: 1`
- `Advanced` -> `Mode` -> `MaxConcurrent: 1`

These settings are important. Without them Dokploy may treat the migrator like a normal long-running application and keep restarting it after it exits successfully.

Webhook behavior:

- keep a single Dokploy webhook for the migrator deployment,
- trigger it only from CI after publishing the `autodor-migrator:latest` image,
- use one shared webhook because both application instances use the same database migration job.

This repository currently stores that webhook in GitHub Actions as:

- `WEBOOK_ULR_MIGRATOR`

If Dokploy requires a build target instead of a prebuilt image, use `Dockerfile.migrator`.

## 14. Configure Environment Variables

Use the repository root `.env.example` as the source of truth for production configuration.

At minimum, review and fill these values in Dokploy:

```env
VITE_CLERK_PUBLISHABLE_KEY=

Authentication__Identity__Authority=
Authentication__Identity__Issuer=
```

Recommended Dokploy layout:

- put shared secrets at the project level when used by multiple services,
- put application-only values at the service level,
- keep database credentials separate from public application settings,
- never hardcode production secrets in the repository.

You must also provide the PostgreSQL connection string expected by the running application under:

```env
ConnectionStrings__Default=Host=<postgres-service-name>;Port=5432;Database=<database-name>;Username=<database-user>;Password=<database-password>
```

The application reads `ConnectionStrings:Default` at startup, so missing this value will prevent the API from booting.

## 15. Add Production Domains for the Application

Once the application service exists, add the public domains in Dokploy.

Typical setup:

- primary domain: `bswierzewski.fun`
- optional subdomain: `www.bswierzewski.fun`
- optional API subdomain if you separate traffic later

For each application domain:

- Host: the public hostname,
- Container Port: `8080`,
- HTTPS: enabled,
- Certificate: `letsencrypt`,
- Path: empty unless you intentionally publish under a subpath.

Dokploy applies application domain changes without requiring a full redeploy, but you should still verify the application after every domain change.

## 16. Configure S3 Destinations

Configure at least one S3 destination in Dokploy before enabling backups.

Prepare:

- bucket name,
- region,
- access key,
- secret key,
- endpoint URL if you use an S3-compatible provider instead of AWS.

Recommended S3 destinations:

- one destination for Dokploy backups,
- one destination for application file storage if the application needs object storage.

Use clear names such as:

- `prod-backups`
- `prod-app-storage`

## 17. Configure Database and Dokploy Backups

There are two backup layers you should configure.

### 17.1 Dokploy Platform Backup

In Dokploy Web Server -> Backups:

1. Create a backup configuration.
2. Choose the S3 destination for backups.
3. Set a schedule using cron syntax.

Dokploy backups include:

- the Dokploy PostgreSQL database,
- the Dokploy filesystem from `/etc/dokploy`.

This protects the deployment platform itself.

### 17.2 Project Database Backup

For the application PostgreSQL database, enable Dokploy database backups on the actual project database service.

Recommendations:

- run at least daily,
- keep multiple restore points,
- periodically test restore on a non-production server,
- document retention, for example 7, 14, or 30 days depending on business requirements.

This repository uses Dokploy-managed Postgres backups instead of a separate backup sidecar container.

## 18. Configure Clerk for Production

Create or switch to a production Clerk instance and set the production application domain.

In Clerk:

1. Set the primary application domain to `bswierzewski.fun`.
2. Configure the production redirect URLs.
3. Configure sign-in and sign-up URLs if you use custom routing.
4. Generate the production publishable key.
5. Generate the production backend secret or signing values required by the API.
6. Update the corresponding Dokploy environment variables.

Important:

- changing the Clerk production domain later can invalidate sessions,
- social login providers must also be updated with the production callback URLs,
- if you use Clerk webhooks, point them to the production Dokploy application URL.

## 19. Add Clerk DNS Records in Cloudflare

After Clerk gives you the production DNS targets, create the required CNAME records in Cloudflare.

Example structure:

```dns
;; CNAME Records
accounts.bswierzewski.fun.      1    IN    CNAME    accounts.clerk.services. ; dns-only
clerk.bswierzewski.fun.         1    IN    CNAME    frontend-api.clerk.services. ; dns-only
clk2._domainkey.bswierzewski.fun. 1 IN CNAME <clerk-dkim-target-2> ; dns-only
clk._domainkey.bswierzewski.fun.  1 IN CNAME <clerk-dkim-target-1> ; dns-only
clkmail.bswierzewski.fun.       1    IN    CNAME    <clerk-mail-target> ; dns-only
```

Rules for these records:

- keep them as DNS-only in Cloudflare,
- copy the exact targets from Clerk,
- do not reuse old DKIM values from another environment unless Clerk explicitly says they are correct,
- wait for Clerk verification to complete before treating the setup as finished.

## 20. Configure CI/CD for Production Deployments

Once the Dokploy panel is available on `dokploy.bswierzewski.fun`, configure CI/CD automation.

Recommended approach:

- build the production images in GitHub Actions,
- push the images to GHCR,
- trigger Dokploy deployment through GitHub repository webhooks after the matching image is published.

Repository-specific CI/CD layout:

- `deploy-app.yml` publishes `ghcr.io/<github-namespace>/autodor-app` with tags `latest` and `<short-sha>`
- `deploy-migrator.yml` publishes `ghcr.io/<github-namespace>/autodor-migrator` with tags `latest` and `<short-sha>`
- both workflows prune GHCR to the last 5 versions
- `deploy-app.yml` triggers only application webhooks
- `deploy-migrator.yml` triggers only the shared migrator webhook `WEBOOK_ULR_MIGRATOR`

Change detection rules used in CI:

- the app workflow runs for changes in `apps/api/**`, `apps/web/**`, `backend/**`, build metadata files, and the app workflow files
- the migrator workflow runs for changes in `apps/migrator/**`, `backend/**/Migrations/**`, build metadata files, and the migrator workflow files
- changes limited to migration folders do not need frontend changes, but backend changes outside migration folders still rebuild the app because the API and migrator compile shared backend modules

Operational notes:

- keep image manifests intact in GHCR,
- avoid cleanup rules that delete required image versions immediately after push,
- when Dokploy is triggered directly from GitHub `registry_package` webhooks, prefer publishing only `latest` to avoid duplicate deploy triggers and tag mismatch errors,
- when multiple Dokploy services consume the same image, add multiple GitHub repository webhooks that point to each Dokploy service trigger,
- keep publishing the `latest` tag if Dokploy services should always pull the newest image automatically,
- prune GHCR to the last few package versions instead of deleting `latest`; the newest kept package version will continue to carry the `latest` tag.

## 21. Post-Deployment Verification Checklist

Verify all of the following before considering the deployment complete:

- `https://dokploy.bswierzewski.fun` opens correctly,
- `https://bswierzewski.fun` opens correctly,
- TLS certificate is valid,
- the Dokploy application routes traffic to container port `8080`,
- the application can connect to PostgreSQL,
- required background jobs and scheduled tasks are enabled if expected,
- SMTP works,
- S3 uploads work,
- Clerk sign-in and sign-up work on the production domain,
- Clerk DNS records are verified,
- CI/CD can trigger a successful redeploy,
- backups are scheduled and a restore procedure is documented.

## 22. Common Pitfalls

Avoid these frequent problems:

- Cloudflare name servers not changed at the registrar level,
- AAAA records missing or pointing to the wrong Mikr.us IPv6,
- Clerk CNAME records accidentally proxied through Cloudflare,
- Dokploy panel domain configured before DNS resolves correctly,
- application domain using the wrong container port instead of `8080`,
- private GHCR token missing package read permissions,
- missing `VITE_CLERK_PUBLISHABLE_KEY` during frontend image build,
- Swarm endpoint mode left at the default instead of `DNS Round Robin` on Mikr.us,
- trying to access Dokploy directly on `IP:3000` on Mikr.us instead of using the SSH tunnel,
- backup schedule configured without first adding an S3 destination.

## 23. Minimum Order of Execution

If you want the shortest safe sequence, follow this order:

1. Buy the domain and Mikr.us server.
2. Add the domain to Cloudflare.
3. Change registrar name servers to Cloudflare.
4. Add root and wildcard AAAA records pointing to the Mikr.us IPv6.
5. Prepare the Mikr.us server and install Dokploy.
6. Access Dokploy through the required PuTTY tunnel on `127.0.0.1:13000`.
7. Publish Dokploy on `dokploy.bswierzewski.fun` with HTTPS.
8. Add registry credentials.
9. Create the project, PostgreSQL database, and application service.
10. Set `DNS Round Robin` for the application endpoint mode.
11. Add production environment variables.
12. Add application domains and verify HTTPS.
13. Configure S3 destinations.
14. Configure Dokploy and database backups.
15. Configure Clerk production.
16. Add Clerk DNS records in Cloudflare.
17. Configure CI/CD webhooks and production deployment automation.
18. Stop using the PuTTY tunnel once the Dokploy domain is working.

## 24. Repository-Specific Notes

This repository currently expects the following production behavior:

- production deployment is Dokploy-first,
- the application should be routed to container port `8080`,
- the frontend is bundled into the API image,
- the frontend build needs `VITE_CLERK_PUBLISHABLE_KEY`,
- Dokploy-managed Postgres backups are preferred over custom backup containers,
- on Mikr.us/LXC, `DNS Round Robin` is the safer Swarm endpoint mode for the application.

Keep this file updated when the production architecture changes.
