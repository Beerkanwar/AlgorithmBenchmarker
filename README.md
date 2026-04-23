# Algorithm Benchmarker

## Project Overview

**Algorithm Benchmarker** is a comprehensive Windows Presentation Foundation (WPF) desktop application developed in C# utilizing the .NET 8.0 framework. Its primary purpose is to provide a clean, extensible, and visually engaging environment for running, benchmarking, and comparing a wide array of computer science algorithms across multiple domains.

This educational and practical tool allows developers and students to measure the time complexity, performance, and behavior of various algorithms dynamically with user-defined inputs or generated datasets.

## Key Features

- **Extensive Algorithm Library:** Support for benchmarking algorithms across 9 distinct categories:
  - **Sorting:** (e.g., Quick Sort, Merge Sort, Bubble Sort, Radix Sort)
  - **Searching:** (e.g., Binary Search, Linear Search, Interpolation Search)
  - **Routing:** Pathfinding and network routing algorithms.
  - **Machine Learning:** Basic ML algorithm implementations.
  - **Indexing:** Data indexing and retrieval structures.
  - **Graph:** Graph traversal and processing (e.g., Dijkstra, BFS, DFS).
  - **Encryption:** Basic cryptographic concepts.
  - **Dynamic Programming:** Classical DP problems (e.g., Knapsack, Fibonacci).
  - **Compression:** Data compression techniques.
- **Dynamic Input Generation:** Automatically generate suitable inputs (arrays, graphs, text) scaled by user-defined constraints.
- **Visual Results:** Tabular and graphical results visualizing the execution time and performance metrics of the algorithms.
- **Light/Dark Mode UI:** Modern user interface with toggleable themes for accessibility and preference.
- **Extensibility:** Easily drop in new algorithms by implementing a single interface without touching the core UI code or benchmarker engine.

### Advanced Research-Grade Profiling Engine
- **Adversarial Input Synthesizer:** Deterministic generation of worst-case pathological inputs to force algorithms into upper bound states `O(W)`.
- **Micro-Operation Execution Tracer:** Low-overhead `[ThreadStatic]` instruction counting for precise algorithmic step analysis.
- **Concurrency & Thread Scaling Profiler (Amdahl Analyzer):** Evaluate parallel efficiency identifying serial execution fractions via synchronized barriers.
- **Dynamic Data-Structure Swapping:** Inject modular generic underlying structures (e.g. Fib/Binary/Pairing Heaps) and test logic isolation.
- **JIT & Tiered Compilation Profiler:** Map Cold-start vs Warm execution inflection points iteratively.
- **Algorithmic Drag Race Mode:** Execute algorithms concurrently on cloned input states guaranteeing completely matched launch epochs minus GC variance.
- **Energy Estimator:** Hardware-agnostic mathematical derivation models projecting equivalent CPU usage into Joules & CO2e output estimates.
- **Cache Locality Analyzer:** Determine sub-algorithm Cache-Line crossing frequency, abstract cache miss probability, and generic data stride locality mathematically mapping cache coherency without OS-level profiling.
- **Theoretical Bound Verification Engine:** Robust R² least-squares regression validation analyzing empirical output matrices against bounded $O(N \log N)$, $O(N^2)$, etc., verifying complexity guarantees.
- **Algorithmic Phase Transition Detector:** High-resolution sweep execution hunting dataset constraint shifts representing emergent gradient discontinuities (exponential transitions).
- **Managed Runtime GC Topology Profiler:** Track implicit generation-level garbage collection pausing translating allocation sizes into overarching distortion heuristics.

## Tech Stack

- **Language:** C# 12
- **Framework:** .NET 8.0 (Windows runtime)
- **UI Architecture:** WPF (Windows Presentation Foundation)
- **Design Pattern:** MVVM (Model-View-ViewModel)

## Installation & Setup

### Prerequisites
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) (Recommended) or [JetBrains Rider](https://www.jetbrains.com/rider/)
- [.NET 8.0 Desktop Runtime / SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

### Steps

1. **Clone the Repository**
   ```bash
   git clone <repository_url>
   cd CSharpMinor/AlgorithmBenchmarker
   ```

2. **Open the Project**
   - Locate the solution file (`.slnx` or `.sln`) or the `AlgorithmBenchmarker.csproj` file.
   - Open it in your preferred IDE (Visual Studio / Rider).

3. **Restore Dependencies**
   The IDE should automatically restore NuGet packages. If not, open the integrated terminal and run:
   ```bash
   dotnet restore
   ```

4. **Build and Run**
   Press `F5` or `Ctrl+F5` to build and launch the application directly from the IDE. Alternatively, use the CLI:
   ```bash
   dotnet build
   dotnet run --project AlgorithmBenchmarker
   ```

## Usage Instructions

1. Launch the application.
2. the left sidebar, navigate through the **Categories** (e.g., Sorting, Graph, Encryption).
3. Select an **Algorithm** from the populated list.
4. Adjust the **Input Size** or configurations on the right pane (e.g., array size, graph complexity).
5. Click **Run Benchmark** to execute the algorithm.
6. The application will compute the time taken and display the performance metrics in the results view. You can compare multiple runs to observe time complexities like $O(N)$, $O(N \log N)$, or $O(N^2)$.

## Project Structure Overview

```text
AlgorithmBenchmarker/
├── Algorithms/         # Core algorithm implementations grouped by category
│   ├── Compression/
│   ├── DynamicProgramming/
│   ├── Encryption/
│   ├── Graph/
│   ├── Indexing/
│   ├── MachineLearning/
│   ├── Routing/
│   ├── Searching/
│   ├── Sorting/
│   └── IAlgorithm.cs   # The core interface every algorithm must implement
├── Converters/         # WPF value converters for UI bindings
├── Data/               # Input generation and dataset logic
├── Models/             # Core data entities (Metrics, Configurations)
├── Services/           # Background services (Benchmarking engine, File I/O)
├── Themes/             # Resource dictionaries for styles, colors, and Light/Dark modes
├── ViewModels/         # MVVM ViewModels handling business and presentation logic
├── Views/              # WPF XAML Views and UI Controls
├── App.xaml            # Application entry point and global resources
└── MainWindow.xaml     # The primary shell/window of the application
```

## Development Workflow

The project is built around the Open-Closed Principle. To add a new algorithm, you **do not** need to modify the UI or the benchmarking engine. 

1. Navigate to the appropriate folder under `Algorithms/` (or create a new category folder).
2. Create a new C# class (e.g., `QuickSort.cs`).
3. Implement the `IAlgorithm` interface:
   ```csharp
   public class QuickSort : IAlgorithm
   {
       public string Name => "Quick Sort";
       public string Category => "Sorting";
       public string Description => "A highly efficient O(N log N) sorting algorithm.";
       
       public void Execute(object input)
       {
           // Implement the algorithm logic here
           // The input is generally provided by the Data generation services
       }
   }
   ```
4. The application uses Reflection (or a DI container) to auto-discover all classes implementing `IAlgorithm` at startup. The new algorithm will automatically appear in the UI categories!

## Deployment Instructions

To create a self-contained, distributable executable package for users who do not have .NET installed:

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

The compiled binaries and `.exe` will be located under:
`AlgorithmBenchmarker/bin/Release/net8.0-windows/win-x64/publish/`

These files can be bundled and distributed directly to users.

## Testing & Contribution

- Ensure that any new algorithm accurately implements the expected time and space complexity.
- Validated inputs are crucial. Implement input type-checking at the start of your `Execute(object input)` method to prevent casting errors.
- Run `dotnet build` and test the application manually to verify that new algorithms are discovered and run without throwing exceptions.
- Ensure the application is warning-free before committing (`dotnet build -warnaserror` is recommended for cleanliness).