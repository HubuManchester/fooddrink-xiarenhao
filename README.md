# BiteRecord - Food & Drink Memory Logger

## Overview
BiteRecord is a cross-platform mobile application developed with .NET MAUI for the Mobile Computing coursework (6G6Z0014). The app allows users to record and manage their restaurant dining experiences, including photos, locations, ratings, and reviews.

## Theme
Food and Drink

## Features
- Add, view, and delete dining records
- Search records by restaurant or dish name
- Camera integration for taking photos
- Location services with reverse geocoding
- Vibration and haptic feedback
- Dark/Light theme switching
- Large text mode for accessibility
- Local data storage with SQLite

## Hardware Features Used
| Hardware | Usage |
|----------|-------|
| Camera | Take photos of food/dishes |
| Location/GPS | Get current location and convert to address |
| Vibration | Feedback on validation errors and save success |
| Haptic Feedback | Button click feedback |
| Text-to-Speech | Voice reading of reviews (implemented) |

## Screenshots
*(Optional: Add screenshots of your app here)*

## Project Structure
```text
BiteRecord/
├── Models/
│   └── BiteRecordModel.cs
├── Services/
│   ├── DatabaseService.cs
│   ├── AccessibilityService.cs
│   └── SpeechService.cs
├── Views/
│   ├── MainPage.xaml
│   ├── AddItemPage.xaml
│   ├── DetailPage.xaml
│   ├── HardwarePage.xaml
│   └── SettingsPage.xaml
└── Platforms/
    └── Android/
        └── AndroidManifest.xml
```

## How to Run
1. Open the solution in Visual Studio 2022
2. Install NuGet package: `sqlite-net-pcl`
3. Select Android as target platform
4. Deploy to emulator or physical device

## Deployment
- Android Phone (OPPO)
- Android Emulator

## GitHub Usage
Regular commits throughout the development cycle with meaningful commit messages.

## Assessment Criteria Covered
| Criterion | Implementation |
|-----------|---------------|
| UI/UX Design & Accessibility (30%) | XAML layouts, dark/light theme, large text mode |
| Use of Mobile Hardware (20%) | Camera, Location, Vibration, Haptic Feedback, TTS |
| Functionality (20%) | CRUD operations, search, validation |
| Validation & Error Handling (10%) | Form validation with error messages and vibration |
| Code Quality (10%) | Clean architecture, separation of concerns, comments |
| Deployment (5%) | Android phone + emulator |
| GitHub Usage (5%) | Regular commits with history |

## Video Demo
*(Link to your screencast on mmutube/Xuexitong)*

## Module Information
- **Module Code**: 6G6Z0014
- **Module Title**: Mobile Computing
- **Assignment ID**: 1CWK100
- **Assessment Title**: Developing a Cross-Platform Mobile App

## Author
[Xiarenhao]
[21906401]

## Date
June 2026
