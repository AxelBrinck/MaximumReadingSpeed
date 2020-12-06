# Reading at maximum speed in .NET.

So what is the fastest way of reading in .NET?

This project is an example about how to implement the fastest reading method.
It is important to know that this can only be done using *unsafe code*.
Do not worry, as this is not unsafe by any means. It just unlocks pointer
capabilities and C-like programming in .NET.

You need to be sure that all resources used are correctly managed, 
as GC (Garbage Collector) will not be checking these memory addresses and you
could end up having memory-leaks and all sort of memory errors.

## Speed
Taking my old 2014 laptop with SSD, and my Google ProtoBuf repository as
references, they were both reading structured data at 31Mb/s.

With unsafe code and going *beyond* .NET, this method reaches 715Mb/s.

```
Write Speed: 228.867Mb/s. Total: 838860800 bytes.
Read Speed: 715.029Mb/s. Total: 838860800 bytes.
```

Tweaking the unsafe section, you will be able to read structured data.
But the down-side is that is not very friendly to use. You may want to stick
with 64-bit or another primitive as your only type in your stream.

Still worth the implementation if your project is IO critical. As the main
benefit of this project is a very low CPU usage with a maximum HDD speed.

## Notes
Important not to use `Stream.Length`, it is the most inefficient way.
Instead, buffer the stream size and compare against that variable.

Maximum reading speed is achieved when compiling in release mode.
`dotnet run --configuration release`.

## Why maximum speed is not your target.
Let's say that you achieved some other method even faster than this.
You are reading a lot of bytes per second, but, is the resulting type matching
your needs?

Casting is expensive even with unsafe code. The .NET decimal constructor
requires four 32-bit integer numbers, and we are getting 64-bit integers.

So if our target are decimal numbers, the real question is: how many decimal
numbers can we read per second? Maybe other method gives us more bytes per
second, but at the time of converting them to the target type the overall
cost is greater than a slower method.