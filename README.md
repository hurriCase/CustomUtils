# Unity Editor Utilities Package

A collection of useful Unity Editor tools to help streamline your development workflow. This package includes utilities for managing scripts, fixing sprite-related issues, and organizing project assets.

## Features

### 1. Flat Scripts Creator
Easily create flat script structures from nested directories.

- Drag and drop folder support
- Automatic unique naming for duplicate files
- Visual feedback for operations
- Progress tracking
- Source and target folder selection via Unity's Object Field

**Access via:** Tools > Flat Scripts Creator

### 2. Sprite Alpha Adder
Convert RGB sprites to RGBA format to enable different compression formats and allow non-power-of-two resolutions in Unity.

Features:
- Project-wide scanning for RGB sprites that need conversion
- Visual preview of sprites requiring conversion
- Batch processing capabilities
- Individual sprite processing
- Preserves original texture settings
- Minimal alpha channel addition for format conversion
- Enables use of different compression settings

**Access via:** Tools > Add Alpha To Sprite

### 3. Sprite Resizer (NPOT Fixer)
Resize sprites to meet power-of-two requirements and optimize for better memory usage.

Features:
- Single sprite or batch processing
- Preview of original and target sizes
- Progress tracking
- Maintains sprite import settings
- Automatic size calculation
- Multiple sprites processing with progress bar

**Access via:** Tools > FixNPOT

## Installation

1. Open your Unity project
2. Copy the package into your Assets folder
3. The tools will be available under the "Tools" menu in Unity Editor

## Usage

### Flat Scripts Creator
1. Open via Tools > Flat Scripts Creator
2. Select source folder containing your scripts
3. Select target folder for the flat structure
4. Click "Create Flat Scripts" or drag and drop folders
5. Check the status message for operation results

### Sprite Alpha Adder
1. Open via Tools > Add Alpha To Sprite
2. Either:
    - Click "Scan Project for Problematic Sprites" to find RGB sprites that need conversion
    - Manually select a sprite to convert
3. Use "Fix" or "Fix All" buttons to process sprites
4. After conversion, you can modify compression settings in the Unity texture importer
5. Check console for operation logs

### Sprite Resizer
1. Open via Tools > FixNPOT
2. Choose between Single Sprite or Folder mode
3. Select your sprite(s) or folder
4. Review the preview of changes
5. Click "Resize Sprite" or "Resize All Sprites"
6. Wait for the process to complete

## Notes
- Always backup your project before using batch processing tools
- Some operations may take time depending on the number of files being processed
- Make sure you have adequate disk space when using the Flat Scripts Creator
- The Sprite Resizer will only process sprites that need resizing