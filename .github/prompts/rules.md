# Coding Rules for Prompts

- List all missing or incomplete features in `missing-features.md` in the solution's folder.
- Add new features using the existing code structure and style.
- Update `README.md` if the feature is relevant.
- Ensure a `.sln` file and `./.vscode/launch.json`(in the root folder with clear configurations) exist for each solution.
- Cleanup all unused code and files.
- Test all features using the correct configuration the launch config.
- After implementation:
  - Remove the feature from `missing-features.md`.
  - Add the feature and date to `change-log.md`.
  - Document and test as needed.
- After validation:
  - Document and test as needed.

# Directory Structure for Solutions

- Each solution folder has the following structure:
  - The solution folder has a `*.sln` file named exactly the same as the folder.
  - The solution file can be executed by Visual Studio.
  - Every solution folder has its correct execution statement for VS Code.
  - The `src` folder contains the source code for the demo.
  - The `missing-features.md` file lists any features that are not implemented in the demo yet.
  - The `README.md` file provides an overview of the demo and instructions on how to run it.
  - The `change-log.md` file documents changes made to the solution, including new features added and dates of changes.
