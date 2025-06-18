# Missing Features for ConsoleApp

## Features not yet implemented

- **README: Usage for test execution:** (implemented)
  - The README does not mention how to run the tests. Add a section for this.
- **Help command output is static:**
  - The help command lists only hardcoded commands. It should list all available commands dynamically, so new commands are auto-included.
- **No integration tests:**
  - There are only unit tests for handlers. Add integration tests for the CLI as a whole (e.g., parsing, error handling, full command flow).

---

**Changelog:**

- Error handling for missing dependencies implemented on 2025-06-18.
- Sample input/output in README implemented on 2025-06-11.
- Clean architecture, class file organization, DI, and MediatR completed on 2025-06-11.
- Unit tests for Program.cs (CLI parsing, error handling) implemented and passing on 2025-06-11.
- Dynamic command discovery (extensibility for new commands) implemented on 2025-06-18.
- Missing features list updated on 2025-06-18.
