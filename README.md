# Lotto Game

A simple lotto game made with a data-oriented, command-based architecture.

- Data is stored in individual records and contains no behavior apart from getters, setters, formatting, etc.
- Services are only a collection of behaviors (functions).
- Highly configurable with json files.

Why? Because I wanted to demonstrate an alternative option to the traditional OOP design, and I also thought it would be fun... it was.

### Cons
- Boilerplate and initial system architecture can be time-consuming.

### Pros
- Once the architecture is set up, it is quite simple to add additional functionality (Behavior, Commands, Result Types, Processing Logic).
- Highly performant, scalable, and parallelizable.
- Command/Event-based architectures allow for more fault-tolerant systems by means of event sourcing.

### Ways to Improve
- More comprehensive testing.
- A tag system that will allow more automated handling of commands (A command could have a tag with a function pointer), thereby eliminating the need to jump into the processor loop.
- Better handling of results and errors.
- More flexibility in the UI to allow for starting a new game, etc.
- Extra features - credit, deposits, special player types (e.g., VIP).

## Conclusion
Although it can take time to get it up and running, with more time, this could be quite a nice and clean solution that is scalable, extensible, and fault-tolerant.

# Install and run requierments
- .Net 9
- .vscode folder should allow for easy project setup, execution, and setup.