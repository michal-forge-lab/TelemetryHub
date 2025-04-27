# 🚀 TelemetryHub

Light‑weight gRPC service for collecting and streaming telemetry events (logs/metrics) between micro‑services.  

---

## 📚 Table of Contents
1. [❓ Why?](#-why)
2. [🧩 Project structure](#-project-structure)
3. [🛠️ Prerequisites](#️-prerequisites)
4. [▶️ Getting started](#️-getting-started)
   * [🖥️ Run the server](#️-run-the-server)
   * [📤 Send a single event](#-send-a-single-event)
   * [📡 Subscribe to the live feed](#-subscribe-to-the-live-feed)
5. [📦 Bulk upload](#-bulk-upload)
6. [🛣️ Road‑map](#️-road-map)
7. [🛠️ Troubleshooting](#️-troubleshooting)

---

## ❓ Why?
* **Centralised telemetry** – one service to collect, fan‑out or persist events.
* **gRPC** – HTTP/2 streaming, low overhead and typed contracts.
* **Shared contract** – single `telemetry.proto` keeps server and clients in sync.
* **Pluggable** – add DB persistence, Kafka or dashboards later.

---

## 🧩 Project structure
```
TelemetryHub
│  TelemetryHub.sln
└─ src
   ├─ TelemetryHub.Shared      # .proto -> generated C# (models + stubs)
   ├─ TelemetryHub.Server      # ASP.NET Core gRPC host
   ├─ TelemetryHub.Client      # SDK / factory for other services
   └─ TelemetryHub.QuickClient # tiny console demo (Subscribe)
```

---

## 🛠️ Prerequisites
* .NET 9 SDK (`dotnet --version` ≥ 9.0.200)
* Windows dev-cert trusted (HTTPS only):
  ```powershell
  dotnet dev-certs https --trust
  ```

---

## ▶️ Getting started

### 🖥️ Run the server
```powershell
cd src/TelemetryHub.Server
# option A – HTTP/2 plaintext (dev‑friendly)
dotnet run                        # listens on http://localhost:5121 (HTTP/2)

# option B – HTTPS (default template)
dotnet run --launch-profile https # listens on https://localhost:7033 (HTTP/2)
```

### 📤 Send a single event
> Quick and dirty using QuickClient
```powershell
cd src/TelemetryHub.QuickClient
# edit Program.cs to uncomment SendEvent snippet, then
dotnet run
```
Or with `grpcurl`:
```bash
grpcurl -plaintext localhost:5121 telemetry.v1.Telemetry/SendEvent \
  -d '{"service":"Postman","timestamp":0,"level":"INFO","message":"It works!"}'
```

### 📡 Subscribe to the live feed
QuickClient already demonstrates server-streaming `Subscribe`:
```powershell
cd src/TelemetryHub.QuickClient
# Program.cs prints every event it receives
dotnet run
```
Anything you `SendEvent` will appear live in that console.

---

## 📦 Bulk upload
```csharp
using var call = client.BulkUpload();
foreach (var ev in events)
    await call.RequestStream.WriteAsync(ev);
await call.RequestStream.CompleteAsync();
await call.ResponseAsync; // ACK
```

---

## 🛣️ Road‑map
| Milestone | Description |
|-----------|-------------|
| 🔧 **Persistence** | Save events in SQLite/PostgreSQL. |
| 📡 **Dashboard**   | Blazor WASM UI streaming `Subscribe`. |
| ♻️ **OpenTelemetry** | Trace gRPC calls & export to Prometheus. |
| 📦 **Docker** | Multi‑stage build & docker-compose. |
| 🚀 **CI/CD** | GitHub Actions – build, test, push image. |

---

## 🛠️ Troubleshooting
| Symptom | Fix |
|---------|------|
| `StatusCode Unavailable` | Ensure server listens on correct protocol / port; QuickClient needs `Http2UnencryptedSupport` for plaintext. |
| `Certificate error` | Run `dotnet dev-certs https --trust` or switch to plaintext profile. |
| Code gen errors | `dotnet clean`, `dotnet restore`; check `<Protobuf Include="Protos/telemetry.proto" GrpcServices="Both" />` in Shared project. |

---

Happy hacking! 🚀
