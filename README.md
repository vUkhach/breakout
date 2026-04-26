# Breakout Game

This project is a feature-rich desktop adaptation of the classic arcade game, Breakout, engineered with C# and WPF (.NET 9). The primary objective is to strategically clear a grid of colorful blocks using a dynamically bouncing ball and a highly responsive, mouse-controlled paddle. Multi-hit blocks are integrated to scale difficulty and add strategic depth to the gameplay.

## 🚀 Technologies Used
* **C# (.NET 9)**
* **WPF & XAML**
* **Entity Framework Core (SQLite)**
* **xUnit & Reflection**
* **Git & GitHub**

## 🎮 Game Features
* **Dynamic Physics:** Fluid, real-time paddle controls and dynamic ball physics complete with bounding-box collision detection.
* **Progressive Difficulty:** Varying block durability (color-coded for 1 to 3 HP) and a progressive scoring system.
* **Survival & Lives:** A dedicated survival timer and a strict limited-lives mechanic.
* **Leaderboard:** Persistent all-time leaderboard storing the top 5 sessions (sorted by score and time).

## ⚙️ Application Architecture & Performance
* **Clean Design:** Rooted in a clean, modular, and object-oriented design, encapsulating core entities (Ball, Paddle, Block). The `MainWindow` seamlessly orchestrates the game loop, physics calculations, and state management.
* **UI Separation:** Declaratively structured via XAML, ensuring a strict separation of presentation and logic with a high-performance Canvas.
* **Database Layer:** Data persistence is elegantly handled via EF Core paired with a lightweight SQLite database using LINQ.
* **Optimized Rendering:** Standard timers are replaced with `CompositionTarget.Rendering` to synchronize natively with the monitor's refresh rate.
* **Delta Time:** Calculations guarantee smooth, frame-rate-independent physics across varying hardware.
* **Unit Testing:** Automated xUnit tests safeguard core mechanics. C# Reflection is employed to safely bypass encapsulation to test private fields.

## 🕹️ Controls
* **Mouse:** Move the cursor horizontally to control the paddle.

## 💻 Getting Started
1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/your-repo-name.git
   ```
2. Open the `breakout.sln` solution file in Visual Studio.
3. Build the project to automatically restore the required NuGet packages (Entity Framework Core, xUnit).
4. Run the application. The SQLite database (`game.db`) will be automatically generated upon the first launch.
