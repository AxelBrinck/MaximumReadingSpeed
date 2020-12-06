# Reading primitive at maximum speed.

## Notes
Important not to use `Stream.Length`, it is the most inefficient way.

Instead, buffer the stream size and compare against that variable.