# 🚀 TelemetryHub

Light‑weight gRPC service for collecting and streaming telemetry events (logs/metrics) between micro‑services.

---

## 📚 Table of Contents
1. [❓ Why?](#-why)
2. [🧩 Project structure](#-project-structure)
3. [🛠️ Prerequisites](#️-prerequisites)
4. [▶️ Getting started](#️-getting-started)
   * [🖥️ Run the server](#️-run-the-server)
   * [🌐 Run the Blazor UI](#-run-the-blazor-ui)
   * [📤 Send a single event](#-send-a-single-event)
   * [📡 Live feed](#-live-feed)
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
```text
TelemetryHub
│  TelemetryHub.sln
└─ src
   ├─ TelemetryHub.Shared      # .proto ➜ generated C# (models + stubs)
   ├─ TelemetryHub.Server      # ASP.NET Core gRPC host
   ├─ TelemetryHub.Client      # SDK / factory for other services
   ├─ TelemetryHub.QuickClient # tiny console demo (Subscribe / SendEvent)
   └─ TelemetryHub.UI          # Blazor WebAssembly live dashboard
```

---

## 🛠️ Prerequisites
* .NET 9 SDK (`dotnet --version` ≥ 9.0.200)
* Trust the dev HTTPS cert (Windows/macOS):
  ```powershell
  dotnet dev-certs https --trust
  ```

---

## ▶️ Getting started

### 🖥️ Run the server
```powershell
cd src/TelemetryHub.Server
# HTTP/2 plaintext (dev‑friendly)
dotnet run                        # http://localhost:5121 (HTTP/2)

# HTTPS profile
dotnet run --launch-profile https # https://localhost:7033 (HTTP/2) + http://localhost:5121
```

### 🌐 Run the Blazor UI
```powershell
cd src/TelemetryHub.UI
dotnet run                         # http://localhost:5252
                                   # navigate to /live
```

### 📤 Send a single event (QuickClient)
```powershell
cd src/TelemetryHub.QuickClient
# Program.cs already contains SendEvent example
dotnet run
```
Or via **grpcurl**:
```bash
grpcurl -plaintext localhost:5121 telemetry.v1.Telemetry/SendEvent \
  -d '{"service":"Postman","timestamp":0,"level":"INFO","message":"It works!"}'
```

### 📡 Live feed
1. Open the UI at `http://localhost:5252/live`  
2. Any `SendEvent` or `BulkUpload` appears instantly in the list.

---

## 📦 Bulk upload (client‑stream)
```csharp
using var call = client.BulkUpload();
foreach (var ev in events)
    await call.RequestStream.WriteAsync(ev);
await call.RequestStream.CompleteAsync();
await call.ResponseAsync; // ACK from server
```

---

## 🛣️ Road‑map
| Milestone | Description |
|-----------|-------------|
| 🔧 **Persistence** | Save events in SQLite/PostgreSQL. |
| 📡 **Dashboard**   | Blazor WASM UI streaming `Subscribe` (✔ basic view). |
| ♻️ **OpenTelemetry** | Trace gRPC calls & export to Prometheus / Jaeger. |
| 📦 **Docker** | Multi‑stage build & docker‑compose. |
| 🚀 **CI/CD** | GitHub Actions – build, test, push image. |

---

## 🛠️ Troubleshooting
| Symptom | Fix |
|---------|------|
| `StatusCode Unavailable` | Ensure server & client use the same protocol/port; QuickClient/Blazor needs gRPC‑Web or `Http2UnencryptedSupport` for plaintext. |
| `NETSDK1082` runtime pack | Do **NOT** reference TelemetryHub.Server in WASM; keep only *TelemetryHub.Shared* reference. |
| `Certificate error` | Run `dotnet dev-certs https --trust` or start server in plaintext mode. |
| Code generation errors | `dotnet clean`, `dotnet restore`; verify `<Protobuf Include="Protos/telemetry.proto" GrpcServices="Both" />` in *Shared*. |

---

Happy hacking and streaming! 🚀

