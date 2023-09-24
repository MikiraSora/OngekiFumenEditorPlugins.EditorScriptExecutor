## 注意
此项目已合并到[宿主项目](https://github.com/NyagekiFumenProject/OngekiFumenEditor)中

## 简介
  本项目为[编辑器](https://github.com/NyagekiFumenProject/OngekiFumenEditor)的插件项目，用于对编辑器添加脚本支持。
  用户可以通过自己编写以及执行脚本，对编辑器以及其他程序运行时资源进行操控。

## 技术栈实现
   * 脚本文件本质是[C# Script](https://docs.microsoft.com/en-us/archive/msdn-magazine/2016/january/essential-net-csharp-scripting)
   * 脚本文件通过[Roslyn](https://github.com/dotnet/roslyn)解析文本/分析语法树/动态编译成in-memory assembly然后执行。
   * 脚本编辑器用的是[AvalonEdit](https://github.com/icsharpcode/AvalonEdit),代码补全用的是Roslyn
   * 生成csproj项目文件用的是[MSBuild](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-api?view=vs-2022)

## 安全提示
  如果你不确定你的脚本的安全性，那就不要加载和执行，后果自负。
  
## 截图
![image](https://user-images.githubusercontent.com/7549173/168473960-0a012d3d-3f17-4169-a206-b756cc3e0628.png)
