# ChatSupport.API

A backend chat session management system designed to handle high volumes of incoming chat requests using a FIFO queue model, round-robin agent assignment, and automated polling monitoring. Developed in alignment with the task specification by Andrew Spiteri (March 2023).

## 🚀 Features

- ✅ FIFO queue for incoming chat requests
- ✅ Round-robin assignment preferring junior → mid → senior → lead
- ✅ Agent concurrency limits respected
- ✅ Office hours enforcement with fallback to overflow team
- ✅ Polling mechanism to track session activity
- ✅ Automatic removal of inactive sessions
- ✅ Manual session removal and agent release
- ✅ Real-time logging of all queue/assignment events

---

## ⚙️ Tech Stack

- .NET 8 Web API
- Hosted BackgroundService for polling
- In-memory queues and session tracking (suitable for demo/test)

---

## 🧪 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- Optional: Swagger UI enabled for local testing

### Run Locally

```bash
git clone https://github.com/Venkadramanan/ChatSupportSystem.git
cd ChatSupport.API
dotnet run
