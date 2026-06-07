# GenesisLoader
第一个（或许吧）为多洛可小镇打造的轻量模组加载器。<br>
对了，并不能加载bepinex的模组/插件。<br>
first(probably) modloader build specific to doloc town.<br>
cant load bepinex plugins/mod, by the way.<br>

## 简体中文

### 安装步骤
第一步: 解压下载到的文件。<br>
第二步: 打开解压后的文件夹。<br>
第三步：把文件夹里面所有的东西放进游戏的根文件夹。<br>
第四步: 打开一次游戏，以让配置文件和文件夹自动生成。<br>

### 配置
此模组在第一次运行的时候会生成GenesisLoader.cfg，目前只可以设置各文件夹的路径。<br>
﻿具体结构:<br>
﻿\[Path]  -配置类别，目前只有路径可供设置。<br>
﻿DataPath = ./GenesisLoader/DolocData -数据文件夹，ContentLoader会在里面放入游戏的JSON数据，但不会加载。<br>
﻿ModPath = ./GenesisLoader/DolocMod -模组文件夹，放在此文件夹里面的模组才会被加载。<br>
﻿LogPath = ./GenesisLoader/Log.txt -日志，调试用，如果报错并确认是模组加载器或插件问题，请发布issue或模组页面上面评论。<br>
﻿AssemblyPath = ./Lib -插件/依赖库文件夹，所有插件会在这里加载。<br>

## English

### How to install
Step 1: unzip the downloaded file.<br>
Step 2: open the unzipped file(a folder).<br>
Step 3: Find the folder containing Genesis.Core.dll, put everything into the game's root folder.<br>
Step 4: Open the game and let the config/folder generates.<br>

### Configs
The mod will generate a config file named "GenesisLoader.cfg". Right now, you can only customize path with it<br>
﻿Structure:<br>
﻿﻿\[Path] -The header of a config<br>
﻿  ﻿DataPath = ./GenesisLoader/DolocData -Folder where read-only game json will dump, modifying them in place have no effect over the game, and will auto generate if missing.<br>
﻿  ﻿ModPath = ./GenesisLoader/DolocMod -The folder where you put mods, only mods in this folder will get loaded.<br>
﻿  ﻿LogPath = ./GenesisLoader/Log.txt -Application logs, for debugging. If you want to report a bug, please make sure its either modloader or plugin issue and report it to github or the corresponding mod page.<br>
﻿  ﻿AssemblyPath = ./Lib -The folder where plugins and dependencies load, put plugins and dependencies here.<br>
