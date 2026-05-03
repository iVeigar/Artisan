using System;
using System.Reflection;
using Wasmtime;

namespace ThiriaExpertSolver.Thiria;

public static class WasmRunner
{
    private static readonly Engine engine = new();
    private static readonly Wasmtime.Module module;
    static WasmRunner()
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("expert.wasm") ?? throw new Exception("找不到嵌入的 expert.wasm 资源，请检查命名空间和文件名是否正确。");
        module = Wasmtime.Module.FromStream(engine, "expert.wasm", stream);
    }

    public static ThiriaSession CreateSession()
    {
        var newStore = new Store(engine);
        var linker = new Linker(engine);
        linker.Define("env", "emscripten_random", Function.FromCallback(newStore, () => new Random().NextSingle()));
        var newInstance = linker.Instantiate(newStore, module);
        return new(newInstance);
    }
}