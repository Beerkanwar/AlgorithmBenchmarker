# Algorithm Benchmarker: User Documentation

Welcome to the Algorithm Benchmarker! This guide explains how to use the **Advanced Research & Profiling Engines** newly integrated into the application interface.

These new tools allow you to measure much more than just "how long did it take." They provide deep insights into how the computer hardware interacts with the exact instructions of the algorithm.

## How to Find the New Features
On the **Run Benchmark** tab, you will notice two new sections:
1. **Execution Mode** dropdown (under Algorithm Selection).
2. **Advanced Research & Profiling Engines** checkbox panel.

---

## 1. Execution Modes

By default, the application runs a **Standard Batch Profiler** (e.g., measuring an array of size 1,000 to 10,000 in steps of 1,000). You can now select two specialized execution modes:

### Drag Race Mode
* **What it does**: If you want to compare two algorithms fairly (e.g., QuickSort vs. MergeSort), the OS processing background tasks can sometimes make one seem slower unfairly. Drag Race Mode drops all selected algorithms onto the exact same cloned dataset, starts them at the *exact same microsecond* using a synchronized barrier, and races them.
* **How to use it**: Select a Category, select "Drag Race Mode", and hit Run. A popup will instantly rank the winners by exact elapsed milliseconds.

### Phase Transition Sweeper
* **What it does**: Algorithms sometimes "break" or slow down exponentially not because the input got larger, but because a *parameter* changed. For example, Graph algorithms might become exponentially slower when a graph crosses 40% density. This sweeper automatically hunts for that exact breaking point.
* **How to use it**: Select "Phase Transition Sweeper" and hit Run. A popup will alert you to the exact density ratio or multiplier where the mathematical curve broke.

---

## 2. Advanced Profiling Checkboxes

When using the **Standard Batch Profiler**, you can toggle any of the following Checkboxes to gather deeper intelligence on your algorithm runs.

When a benchmark finishes with these boxes checked, click **VIEW RESULTS** and expand the **Detailed Results Table** at the bottom of the screen. Scroll to the far right to see the new **Extended Metrics** column, containing the captured mathematical values separated by a `|` symbol.

### Adversarial Input Generator
Instead of sorting random numbers, this purposefully feeds the algorithm pathological, "worst-case" data explicitly designed to break it (acting as a stress test for $O(W)$ states).

### Micro-Operation Tracer
System clocks are unreliable due to CPU power scaling. This tracer ignores time entirely and perfectly counts the exact number of logical instructions the program performed (e.g., precise number of data swaps or exact variable comparisons).

### Amdahl Concurrency Profiler
Tests parallel algorithms to see how efficient they truly are. It mathematically extracts the "Serial Fraction" (the part of the code that *cannot* be sped up by adding more CPU cores). 

### Dynamic Container Swapping
Swap out the underlying fundamental data structure of a generic algorithm (e.g., making a routing algorithm use a Fibonacci Heap instead of a standard Array) to analyze the performance difference caused strictly by the data model.

### JIT Warmup Profiler
The .NET runtime is "smart" and makes code run faster over time by analyzing it (Tiered Compilation). This profiler tracks the execution and reports the exact iteration where the compiler "woke up" and massively sped your code up.

### GC Topology Profiler
Tracks "Garbage Collection." Complex code throws away a lot of unused variables. This tracks how many times the computer had to pause your algorithm to safely delete that unused memory (Gen 0/1/2 pauses), outputting a total Pressure Score.

### Energy & Carbon Estimator
Mathematically analyzes the CPU cycles and memory allocations required by your algorithm and converts them into an estimate of literal hardware electrical consumption (Joules) and Carbon footprint (CO2e Grams).

### Cache Locality Analyzer
CPUs have very fast, very small memory chips called "Caches". If your algorithm reads memory out of order (like traversing a Linked List), it misses this cache and runs up to 100x slower. This analyzer scores your algorithm out of 1.0; closer to 1.0 means your algorithm utilizes cache coherency flawlessly.

### Theoretical Complexity Verifier
Ever wonder if your code is *actually* $O(N \log N)$? This engine takes your empirical timing data across the batch array and runs a Least-Squares statistical regression against known equations, proving definitively which mathematical model fits your real-world outcomes best.
