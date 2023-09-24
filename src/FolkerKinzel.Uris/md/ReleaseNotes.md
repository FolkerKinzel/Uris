- Dependency update.

>**Notes to users who build .NET Core 2.x/3.0 applications based on the .NET Standard 2.0 part of the package:**  
>You might get a compiler error. This is caused by a Microsoft dependency. You can get rid of this error, if you copy `<SuppressTfmSupportBuildWarnings>true</SuppressTfmSupportBuildWarnings>` to a `<PropertyGroup>` of your project file (at own risk).

>**Project reference:** On some systems, the content of the CHM file in the Assets is blocked. Before opening the file right click on the file icon, select Properties, and check the "Allow" checkbox - if it is present - in the lower right corner of the General tab in the Properties dialog.