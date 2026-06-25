#  Cybersecurity Awareness Bot (POE Part 3)

A Windows Forms chatbot built with C# and .NET 8 that helps users learn about cybersecurity, manage security tasks, take quizzes, and track activity.

## Features

* 💬 Cybersecurity chatbot
* 🎙️ Voice greeting on startup
* 😊 Sentiment detection
* 🧠 User memory and conversation tracking
* 📋 Task Assistant (Add, View, Complete, Delete tasks)
* 🗄️ MySQL database integration
* 🔔 Task reminders
* 🎯 Cybersecurity quiz mini-game
* 🧠 NLP intent recognition
* 📜 Activity log with timestamps

## Technologies Used

* C#
* .NET 8
* Windows Forms (WinForms)
* MySQL
* GitHub

## Project Structure

```text
CybersecurityBotPOE/
├── Forms/
├── Database/
├── Tasks/
├── Quiz/
├── NLP/
├── ActivityLog/
├── Memory/
├── Sentiment/
├── Responses/
├── Helpers/
└── Program.cs
```

## Setup

### Prerequisites

* Visual Studio 2022
* .NET 8 SDK
* MySQL Server

### Configure MySQL

Update the connection string in `DatabaseManager.cs`:

```csharp
Server=localhost;Database=cybersecurity_bot;Uid=root;Pwd=YOUR_PASSWORD;
```

### Run the Project

1. Open the solution in Visual Studio.
2. Restore NuGet packages.
3. Add `greeting.wav` to the project folder.
4. Press **F5** to run.

## Database Table

```sql
CREATE TABLE tasks (
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description TEXT NOT NULL,
    reminder VARCHAR(255),
    is_complete TINYINT(1) DEFAULT 0,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);
```

## Quiz Topics

* Phishing
* Password Security
* Malware
* Multi-Factor Authentication
* Social Engineering
* Safe Browsing

## Author
Phindulo Nekhofhe

Programming of Event-Driven Applications (POE)

## Video Demonstration

 YouTube presentation link here: https://youtu.be/AkPScGmzuhI


