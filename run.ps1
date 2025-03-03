﻿#Requires -Version 5

using namespace System
using namespace System.IO
using namespace System.Management.Automation

[CmdletBinding(PositionalBinding = $false)]
param
(
    [Parameter()]
    [switch] $Clean,

    [Parameter()]
    [switch] $NoBenchmarking,

    [Parameter()]
    [switch] $AppveyorBuild
)
begin
{
    $Script:ErrorActionPreference = [System.Management.Automation.ActionPreference]::Stop
    Microsoft.PowerShell.Core\Set-StrictMode -Version 1

    function Write-MajorSeparator
    {
        [CmdletBinding(PositionalBinding = $false)]
        param ()
        process
        {
            Write-Host ''
            Write-Host -ForegroundColor Magenta ('=' * 100)
            Write-Host ''
        }
    }

    function Write-ActionTitle
    {
        [CmdletBinding(PositionalBinding = $false)]
        param
        (
            [Parameter(Position = 0, Mandatory = $true, ValueFromPipeline = $true)]
            [ValidateNotNullOrEmpty()]
            [string] $Title
        )
        process
        {
            Write-Host -ForegroundColor Green $Title
        }
    }

    function Get-ProjectItemFullPath
    {
        [CmdletBinding(PositionalBinding = $false)]
        param
        (
            [Parameter(Position = 0, Mandatory = $true, ValueFromPipeline = $true)]
            [ValidateNotNullOrEmpty()]
            [string] $RelativePath
        )
        process
        {
            [ValidateNotNullOrEmpty()] [string] $result = [Path]::GetFullPath([Path]::Combine($PSScriptRoot, $RelativePath))
            return $result
        }
    }

    function Test-ExitCode
    {
        [CmdletBinding(PositionalBinding = $false)]
        param ()
        process
        {
            if ($LASTEXITCODE -ne 0)
            {
                throw "The command failed with exit code $LASTEXITCODE."
            }
        }
    }
}
process
{
    [ValidateNotNullOrEmpty()] [string] $solutionFilePath = 'HarinezumiSama.Benchmarks.sln' | Get-ProjectItemFullPath
    [string[]] $outputDirectories = @('.out', '.benchmarks') | Get-ProjectItemFullPath

    [string] $executableProjectName = 'HarinezumiSama.Benchmarks.Executor'

    [ValidateNotNullOrEmpty()] [string] $executableFilePath = `
        ".out/bin/AnyCPU/Release/$executableProjectName/net9.0/$executableProjectName.exe" | Get-ProjectItemFullPath

    [ValidateNotNullOrEmpty()] [string] $executableDirectoryPath = [Path]::GetDirectoryName($executableFilePath)

    Write-MajorSeparator
    Write-ActionTitle '.NET SDKs'
    dotnet --list-sdks

    if ($Clean)
    {
        Write-MajorSeparator
        Write-ActionTitle "Cleaning build output for the solution ""$solutionFilePath""."
        dotnet clean --verbosity normal """$solutionFilePath""" 2>&1

        Write-Host ''
        Write-ActionTitle 'Deleting output directories (if any).'
        foreach ($outputDirectory in $outputDirectories)
        {
            if (Test-Path -LiteralPath $outputDirectory)
            {
                Write-Host ''
                Write-ActionTitle "Deleting output directory ""$outputDirectory""."
                Remove-Item -Path $outputDirectory -Exclude '.gitkeep' -Recurse -Force -ErrorAction SilentlyContinue
            }
        }
    }

    Write-MajorSeparator
    Write-ActionTitle "Restoring packages for the solution ""$solutionFilePath""."
    dotnet restore --verbosity normal """$solutionFilePath""" 2>&1
    Test-ExitCode

    [string[]] $buildCommandExtraArguments = @()
    if ($AppveyorBuild)
    {
        [ValidateNotNullOrEmpty()] [string] $buildVersion = $env:APPVEYOR_BUILD_VERSION
        [ValidateNotNullOrEmpty()] [string] $branch = $env:APPVEYOR_REPO_BRANCH
        [ValidateNotNullOrEmpty()] [string] $revisionId = $env:APPVEYOR_REPO_COMMIT

        [string] $shortRevisionId = $revisionId.Substring(0, 12)
        [string] $timestamp = [datetime]::UtcNow.ToString('yyyyMMdd"T"HHmmss"Z"')

        $buildCommandExtraArguments += `
            @(
                "-p:Version=""$buildVersion"""
                "-p:InformationalVersion=""$buildVersion+$shortRevisionId.$branch.$timestamp"""
            )
    }

    Write-MajorSeparator
    Write-ActionTitle "Building the solution ""$solutionFilePath""."
    dotnet build --verbosity normal """$solutionFilePath""" --no-incremental --no-restore @buildCommandExtraArguments 2>&1
    Test-ExitCode

    Write-MajorSeparator
    Write-ActionTitle "Running tests for the solution ""$solutionFilePath""."
    dotnet test --verbosity normal """$solutionFilePath""" --no-build --logger:console --logger:html --logger:trx 2>&1
    Test-ExitCode

    Write-MajorSeparator
    Write-ActionTitle "Listing benchmarks using ""$executableFilePath""."
    Push-Location $executableDirectoryPath
    try
    {
        & $executableFilePath --list flat 2>&1
        Test-ExitCode
    }
    finally
    {
        Pop-Location
    }

    if ($NoBenchmarking)
    {
        Write-MajorSeparator
        Write-Warning -WarningAction Continue 'Not executing benchmarks.'
        Write-MajorSeparator
        return
    }

    Write-MajorSeparator
    Write-ActionTitle "Executing benchmarks using ""$executableFilePath""."
    Push-Location $executableDirectoryPath
    try
    {
        & $executableFilePath 2>&1
        Test-ExitCode
    }
    finally
    {
        Pop-Location
    }

    Write-MajorSeparator
}