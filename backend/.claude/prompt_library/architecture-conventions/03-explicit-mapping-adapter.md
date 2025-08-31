### 🎯 RULE: Explicit Mapping Adapter

**Rule:** The API implementation class in `Infrastructure/Api` acts as a simple adapter. Its sole responsibility is translation. Methods MUST follow this sequence:
1.  Receive parameters from the public method call.
2.  Create a new, internal `Query` or `Command` object (`var query = new SpecificInternalQuery(...)`).
3.  Send it using `MediatR` (`var result = await _sender.Send(query, ct)`).
4.  Map the internal response to a public DTO and return it.