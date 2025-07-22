# JirAutomate

This app lets users upload a meeting transcript, extract actionable tasks using Google Gemini AI, and automatically create Jira tickets — all from a web UI built in Angular + .NET.
It also includes Ticket Management Dashboard to edit extracted tickets, change assignees. etc..

---

## ⚙️ Tech Stack

- ✅ Angular 20
- ✅ .NET 8 Web API (C#)
- ✅ Google Gemini API (LLM)
- ✅ Jira Cloud REST API
- ✅ MongoDB
- ✅ JWT

---

## 🚀 Features

- Upload meeting transcripts
- Gemini extracts action items as task or bug
- Automatic mapping of names → Jira users
- Jira tickets created directly from UI

---

## 🛠️ Local Setup

### 1. Clone the repo

```bash
git clone https://github.com/swapnilgarg7/jirautomate.git
cd jirautomate
```

---

### 2. Backend Setup (.NET API)

```bash
cd JirAutomate
dotnet restore
dotnet run
```

#### 🔐 Required `.env` (in `JirAutomate/`)

```
GEMINI_API=your_google_api_key
```

#### ✅ Runs at: `http://localhost:5208`

---

### 3. Frontend Setup (Angular)

```bash
cd frontend
npm install
ng serve
```
#### 🔐 Required `environment.ts` (in `frontend/src/environments/`)

```
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5208/api'
};
```

#### ✅ Runs at: `http://localhost:4200`

---

## 📦 API Endpoints

| Method | Endpoint                  | Description                         |
|--------|---------------------------|-------------------------------------|
| POST   | `/api/transcript/upload`  | Upload meeting transcript           |
| POST   | `/api/jira/create-ticket` | Create Jira ticket manually         |
| POST   | `/api/auth/register`      | Register a user with Jira details   |
| POST   | `/api/auth/login`         | Create Jira ticket manually         |

---

## ✨ Coming Soon

- Enter names and atlassian emails to assign assignees to the tickets
- Other files support

---
