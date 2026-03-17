# Project Blueprint

## Overview

This project is a full-stack web application with a .NET backend and a React frontend. The goal is to create a robust and scalable application by leveraging ASP.NET Core for the backend and React for a modern, interactive user experience.

## Project Structure

*   **Backend:** The .NET backend is located in the root directory. It follows the standard ASP.NET Core project structure.
*   **Frontend:** The React frontend is located in the `client-app` directory.

## Completed Task: Integrate React Frontend

I have successfully integrated the React frontend with the .NET backend. Here's a summary of the changes:

*   **Isolated the Frontend:** The `client-app` is now a pure frontend application. The Express.js server has been removed.
*   **Configured Vite:** A `vite.config.ts` file has been added to the `client-app` to manage the development server and build process. It includes a proxy to the .NET backend.
*   **Updated `Program.cs`:** The `Program.cs` file now serves the React application's static files in production and uses controllers for API endpoints.
*   **Added Example API:** A `WeatherForecastController` has been added to demonstrate backend communication.

## How to Run the Application

To run the application, you'll need to open two terminals:

**Terminal 1: Run the .NET Backend**

```bash
dotnet run
```

This will start the .NET backend on port `5000`.

**Terminal 2: Run the React Frontend**

```bash
cd client-app
npm run dev
```

This will start the React development server on port `3000`.

You can now access your application at `http://localhost:3000`. Any API requests made from the React app will be proxied to the .NET backend.