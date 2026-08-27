# ResumeIQ - Enterprise ATS Analysis Platform

ResumeIQ is a cloud-native, decoupled ASP.NET Core Web API and MVC application designed to revolutionize resume parsing. By integrating Google's Gemini 3.6 Flash model with strict zero-shot prompt engineering, ResumeIQ moves beyond basic keyword matching to provide intelligent, bias-free applicant evaluation.

## 🚀 Key Features

*   **AI Substantiation Engine:** Actively combats keyword-stuffing by cross-referencing listed technical skills against quantitative project metrics.
*   **Explainability Node (Responsible AI):** Provides a transparent, bias-free rationale for every scoring deduction, eliminating the "black box" nature of traditional ATS systems.
*   **Strict JSON Schema Enforcement:** Utilizes advanced prompt engineering to force deterministic, parsable JSON outputs from the LLM, ensuring data integrity.
*   **Resilient Architecture:** Features graceful UI degradation with `try-catch` deserialization blocks to prevent server crashes during LLM hallucinations.
*   **Secure Infrastructure:** Implements ASP.NET Core Identity middleware (`[Authorize]`) and strictly externalized API credentials to protect cloud resources.

## 🛠️ Technology Stack

*   **Backend:** C#, ASP.NET Core Web API, Entity Framework Core (In-Memory Seeding)
*   **AI Integration:** Google GenAI SDK (Gemini 3.6 Flash)
*   **Frontend:** ASP.NET Core MVC (Razor Pages), Bootstrap 5, Vanilla JS (Asynchronous state management)
*   **Data Parsing:** System.Text.Json (Dynamic DOM parsing)

## ⚙️ Architecture Overview

The application utilizes a clean separation of concerns. The backend Web API handles HTTP request routing, document ingestion via `IFormFile`, and secure communication with the Google Gemini API. The frontend MVC utilizes dynamic asynchronous states to prevent request duplication and renders the parsed JSON response into interactive evaluation charts.
