
WPF控件库，包含丰富的UI组件，为WPF应用程序提供现代化的界面元素。

## 控件预览

### 基础控件

![基础控件](https://github.com/shaoyidong/ShydControls.Wpf/blob/master/Images/基础控件.gif)

### 光圈

![光圈](https://github.com/shaoyidong/ShydControls.Wpf/blob/master/Images/光圈.gif)

### Dashboards 控件

![仪表盘1](https://github.com/shaoyidong/ShydControls.Wpf/blob/master/Images/仪表盘1.png)
![仪表盘2](https://github.com/shaoyidong/ShydControls.Wpf/blob/master/Images/仪表盘2.png)
![组合仪表盘](https://github.com/shaoyidong/ShydControls.Wpf/blob/master/Images/组合仪表盘.png)

### 图表控件

![图表](https://github.com/shaoyidong/ShydControls.Wpf/blob/master/Images/图表.gif)

### 工业组件

![工业](https://github.com/shaoyidong/ShydControls.Wpf/blob/master/Images/工业.gif)


## 功能特点

- **Charts**: 图表控件，包括五角图、温度风扇曲线等
- **Aperture**: 光圈相关控件，如环形进度条、发光环等
- **Dashboards**: 仪表盘控件，如温度表、风扇速度表、PWM表等
- **IndustrialComponents**: 工业组件，如阀门、管道、冷却泵等
- **Others**: 其他控件，如骨架屏等

## 安装方法

通过NuGet安装：

```powershell
Install-Package ShydControls.Wpf
```

或者使用.NET CLI：

```bash
dotnet add package ShydControls.Wpf
```

## 使用方法

### XAML中引用

只需添加命名空间引用，即可使用所有控件：

```xaml
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:shyd="http://schemas.shydcontrols.com/wpf"
        xmlns:shydIndustrial="http://schemas.shydcontrols.com/wpf/IndustrialComponents"
        xmlns:shydOthers="http://schemas.shydcontrols.com/wpf/Others">       
        
    <!-- 使用Charts命名空间下的控件 -->
    <shyd:TemperatureFanCurveControl />
    
    <!-- 使用Aperture命名空间下的控件 -->
    <shyd:CircularProgressBar />
    
    <!-- 使用Dashboards命名空间下的控件 -->
    <shyd:FanSpeedGauge />
    
    <!-- 使用IndustrialComponents命名空间下的控件 -->
    <shydIndustrial:Valve />
    
    <!-- 使用Others命名空间下的控件 -->
    <shydOthers:SkeletonScreen />
</Window>
```

### C#代码中使用

在C#代码中，您可以根据需要导入相应的命名空间：

```csharp
using ShydControls.Wpf;
using ShydControls.Wpf.Charts;
using ShydControls.Wpf.Aperture;
using ShydControls.Wpf.Dashboards;
using ShydControls.Wpf.IndustrialComponents;
using ShydControls.Wpf.Others;
```

## 支持的框架

- .NET 8.0 Windows
- .NET 6.0 Windows
- .NET Framework 4.8.1
- .NET Framework 4.8
- .NET Framework 4.7.2

## 示例

请参考Demo项目以了解如何使用各个控件。

## 许可证

MIT
