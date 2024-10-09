# Gaze Tracker for VRChat Worlds

This package provides a gaze tracking system for VRChat worlds. It allows you to track player gazes at specific objects in your scene and visualize the data over time.

In theory this is generally useful, but it's currently hardcoded for 6
screen-like objects for a specific other project.

## Features

- Track player gazes at up to 6 different objects
- Network-synced gaze counters
- Time series visualization of gaze data
- Customizable update rate and visualization settings

## Installation

1. Add [the VRChat VPM package](https://hiinaspace.github.io/gaze-tracker-vrc/)
   to your project with the Vrchat Creator Companion. Or find the latest release on
   github and get the .unitypackage version.

## Usage

1. Add the GazeTracker prefab to your world scene
2. Position the 6 GazeTrackColliders to cover the objects you want to track in your scene
   - Ensure the Z forward axis of each collider points towards the front of the object
3. Place the time series plot in a visible location in your scene

## Components

### GazeTracker

The main component that manages the gaze tracking system. It updates gaze data at a specified interval and updates the time series visualization.

### GazeTrackCollider

Attached to box colliders to detect player gazes. It uses raycasting to determine if a player is looking at the front face of the collider.

### TimeSeriesShader

A custom shader used to visualize the gaze data over time. It creates a scrolling graph of gaze counts for each tracked object.

## Customization

- Adjust the `updateRate` in the GazeTracker component to change how often gaze data is collected and visualized
- Modify the colors and other properties in the TimeSeriesShader material to customize the appearance of the visualization
