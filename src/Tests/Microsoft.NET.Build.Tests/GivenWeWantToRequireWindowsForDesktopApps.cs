// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Xml.Linq;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.NET.Build.Tasks;
using Microsoft.NET.TestFramework;
using Microsoft.NET.TestFramework.Assertions;
using Microsoft.NET.TestFramework.Commands;
using Microsoft.NET.TestFramework.ProjectConstruction;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.NET.Build.Tests
{
    public class GivenWeWantToRequireWindowsForDesktopApps : SdkTest
    {
        public GivenWeWantToRequireWindowsForDesktopApps(ITestOutputHelper log) : base(log)
        {
        }

        [WindowsOnlyTheory]
        [InlineData("UseWPF")]
        [InlineData("UseWindowsForms")]
        public void It_builds_on_windows_with_the_windows_desktop_sdk(string uiFrameworkProperty)
        {
            const string ProjectName = "WindowsDesktopSdkTest";

            var asset = CreateWindowsDesktopSdkTestAsset(ProjectName, uiFrameworkProperty);

            var command = new BuildCommand(Log, Path.Combine(asset.Path, ProjectName));

            command
                .Execute("/restore")
                .Should()
                .Pass();
        }

        [PlatformSpecificTheory(Platform.Linux, Platform.Darwin, Platform.FreeBSD)]
        [InlineData("UseWPF")]
        [InlineData("UseWindowsForms")]
        public void It_errors_on_nonwindows_with_the_windows_desktop_sdk(string uiFrameworkProperty)
        {
            const string ProjectName = "WindowsDesktopSdkErrorTest";

            var asset = CreateWindowsDesktopSdkTestAsset(ProjectName, uiFrameworkProperty);

            var command = new BuildCommand(Log, Path.Combine(asset.Path, ProjectName));

            command
                .Execute("/restore")
                .Should()
                .Fail()
                .And
                .HaveStdOutContaining(Strings.WindowsDesktopFrameworkRequiresWindows);
        }

        [WindowsOnlyTheory]
        [InlineData("Microsoft.WindowsDesktop.App")]
        [InlineData("Microsoft.WindowsDesktop.App.WindowsForms")]
        [InlineData("Microsoft.WindowsDesktop.App.WPF")]
        public void It_builds_on_windows_with_a_framework_reference(string desktopFramework)
        {
            const string ProjectName = "WindowsDesktopReferenceTest";

            var asset = CreateWindowsDesktopReferenceTestAsset(ProjectName, desktopFramework);

            var command = new BuildCommand(Log, Path.Combine(asset.Path, ProjectName));

            command
                .Execute("/restore")
                .Should()
                .Pass();
        }

        [PlatformSpecificTheory(Platform.Linux, Platform.Darwin, Platform.FreeBSD)]
        [InlineData("Microsoft.WindowsDesktop.App")]
        [InlineData("Microsoft.WindowsDesktop.App.WindowsForms")]
        [InlineData("Microsoft.WindowsDesktop.App.WPF")]
        public void It_errors_on_nonwindows_with_a_framework_reference(string desktopFramework)
        {
            const string ProjectName = "WindowsDesktopReferenceErrorTest";

            var asset = CreateWindowsDesktopReferenceTestAsset(ProjectName, desktopFramework);

            var command = new BuildCommand(Log, Path.Combine(asset.Path, ProjectName));

            command
                .Execute("/restore")
                .Should()
                .Fail()
                .And
                .HaveStdOutContaining(Strings.WindowsDesktopFrameworkRequiresWindows);
        }

        private TestAsset CreateWindowsDesktopSdkTestAsset(string projectName, string uiFrameworkProperty)
        {
            const string tfm = "netcoreapp3.0";

            var testProject = new TestProject()
            {
                Name = projectName,
                TargetFrameworks = tfm,
                IsSdkProject = true,
                ProjectSdk = "Microsoft.NET.Sdk.WindowsDesktop",
                IsWinExe = true,
            };

            testProject.AdditionalProperties.Add(uiFrameworkProperty, "true");

            return _testAssetsManager.CreateTestProject(testProject);
        }

        private TestAsset CreateWindowsDesktopReferenceTestAsset(string projectName, string desktopFramework)
        {
            const string tfm = "netcoreapp3.0";

            var testProject = new TestProject()
            {
                Name = projectName,
                TargetFrameworks = tfm,
                IsSdkProject = true,
                IsWinExe = true,
            };

            testProject.FrameworkReferences.Add(desktopFramework);

            return _testAssetsManager.CreateTestProject(testProject);
        }
    }
}