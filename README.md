# taishan


## 编译环境

1. 安装 Visual Studio 2019
2. 必须安装以下几个组件
    ```
   .NET 桌面开发
   C++ 桌面开发
   .NET Core 跨平台开发
   Windows 10 SDK
   .NET Core 3.1 运行时
   .NET Framework 4.6.1
    ```
3. 克隆项目
   git clone https://gitlab.com/yzapp/taishan.git
4. 使用 VisualStudio2019 打开项目目录下的 taishan2019.sln
5. 使用以下三种方式之一方式编译解决方案
   - 按 `F6` 快捷键
   - 点击 `菜单栏->生成->生成解决方案`
   - 在解决方案管理器中右键单击解决方案，在弹出的菜单中点击 `生成解决方案`

## 目录说明

    |--taishan
        |--FW                                                   // 存放手柄的固件
        |--demo                                                 // 一些功能的demo项目
        |--doucument                                            // 项目开发过程中的一些文档
        |--update                                               // 升级包，发布文件
        |--yz.gaming.SHA256Generator                            // SHA256验证码计算工具
        |--yz.gaming.accessoryapp                               // APP主程序
            |
            |--Api                                              // APP底层功能API接口
            |--Controls                                         // 自定义控件
            |--Languange                                        // 多语言支持
            |--Model                                            // 数据模型类
            |--Resource                                         // 图片资源与样式
            |--Service                                          // App的一些功能服务类
            |--Utils                                            // 工具类
            |--View                                             // View层，UI相关
            |--ViewModel                                        // ViewModel层，为UI提供数据与业务逻辑处理
            |
        |--yz.gaming.accessoryservice                           // 用于调起APP的服务                  
        |--yz.gaming.com                                        // 手柄驱动底层Lib
        |--yz.gaming.install                                    // 打包安装包
        |--yz.gaming.test                                       // 手柄驱动测试项目
        |--yz.gaming.testcsharp                                 // C#下WMI功能的测试项目
        |--yz.gaming.testopenappviaservice                      // 测试使用服务启动APP
        |--yz.gaming.utils                                      // WIN功能相关，已经整合到APP主程序

## 发布说明

### 1. 修改发布配置并发布App
- 修改解决方案配置为 `Release`
- 在 `解决方案管理器中` 选择 `yz.gaming.accessoryapp` 项目点击右键，在弹出的 `右键菜单` 中选择 `发布`
- 确认发布配置，点击 `发布` 按钮，等待项目发布完成
### 2. 修改安装包的ProductCode
- 在 `解决方案管理器中` 选择 `yz.gaming.install` 项目，再打开菜单中的 `视图->属性窗口`
- 在 `属性` 选项卡中，修改 `Version` 属性为新的版本号，修改完成后，会提示是否同时修改 `ProductCode` 点击是，生成新的 [ProductCode]
- 复制上一步生成的 `ProductCode`，在 `解决方案管理器中` 选择 `yz.gaming.install` 项目点击右键，在弹出的 `右键菜单` 中选择 `View->文件系统`
- 在弹出的编辑窗口中，分别在目录 `Application Folder` 与 `User's Programs Menu -> GameAssistan` 中找到 `Uninstall.exe` 类型为 `Shortuct` 的对象，选中后打开 `属性窗口`，并修改 `Arguments` 参数为新生成的 [ProductCode]，类似 `/x {4CD94096-3E08-4BA5-85C0-203D70E5AB81}`
### 3. 生成安装包
- 在 `解决方案管理器中` 选择 `yz.gaming.install` 项目点击右键，在弹出的 `右键菜单` 中选择 `重新生成`
- 等待项目 `生成` 完成，进入目录 `\yz.gaming.install\Release` 将 `GameAssistant.msi` 与 `setup.exe` 打包为 `GameAssistant_vx.x.xx.x.zip` 压缩包
### 4. 修改在线升级配置
- 将压缩包拷贝到目录 `\taishan\update` 并使用 `yz.gaming.SHA256Generator` 对压缩包生成 `SHA256摘要`
- 使用任意文档编辑工具（推荐 `vscode` 其他编辑器有可能因为编码问题造成APP升级时无法正确的读取文件）打开 `\taishan\update\update.json`
- 将压缩包的 `SHA256摘要` 复制到到配置项目 `sha256` 中，并将 `url` 配置项链接中的文件名修改为新的文件名，`version` 修改为新的版本号，发布历史按需修改
- 提交修改到仓库，运行APP验证是否有正确的更新提示