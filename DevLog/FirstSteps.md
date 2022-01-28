# Writing a blogging engine - First steps

Recently I started thinking about starting a blog. This thought came to me after listening to tech podcasts, and reading tech blogs, all suggesting blogging as a good way to advance your career, or continue learning. So why not start a blog on some topics I'm interested in, it won't hurt me, and who knows, I might be some help to someone out there reading this.

As a first step I was trying to find some blogging engine or platform to host my blog. There are plenty of options, some relatively simple, some complex, some free, some paid. Not really sure what to pick or even how to host the thing I would pick, I started thinking of writing something new. This would surely be a great learning opportunity, and I could write some post while writing the software. Yeah, you're right, I'll be reinventing a wheel that has been reinvented a thousand times, but hey why not?

The goal: write an asp.net core blogging engine, while at the same time writing a series of blog posts detailing the process. Seems simple enough. Wut where to start? First off, in the blog posts I'll be aiming to detail some decisions and why I made these decisions. The full code is available at [my GitHub](https://github.com/ivove/JEWA-Blog). For the actual blogging engine, I took a lot of inspiration from [Miniblog.Core](https://github.com/madskristensen/Miniblog.Core), written by Mads Kristensen. I certainly wanted to avoid using some kind of database engine, primarily to keep hosting cost low. So I was drawn to the idea of storing the blog posts in files, as Mads is doing. At some point however I might implement things a bit different. So Build a blogging engine using files for storage and have a nice frontend to display the posts. Let's get started! First steps, setting everything up.

## GitHub

The project was intended to be open-source, and I needed a way to track my work. The no-brainer solution, set up a git repo on GitHub. An easy enough step following the wizard... The only real decisions to make are the name for the repo, and which license to use. As the name is a bit personal, it does not matter all that much. The license however has an impact. Me, not being the licensing expert, made the choice to go for an MIT license. As this project is purely for learning purposes, I don't mind making the code freely available. If you're going to build something, and you do mind, please read up on licensing. After creating the repo on GitHub, cloning the repo on my developer machine was the next step...

`git clone git@github.com:ivove/JEWA-Blog.git`

That nicely created a local repo to work in. Now just adding a "src" folder for the source and a "DevLog" folder to contain the first series of blog posts. Ready to start creating the project in Visual Studio

## Visual Studio

Visual Studio 2022 community edition is the version installed on my developer machine, so that what will be used. This version allows the use of .NET 6 which will be the .NET version used for the blogging engine. In the wizard choosing create new project and in the next screen choosing to create an ASP.NET core Web App (Model-View-Controller). In the next screen naming the project and choosing the location, making sure the location corresponds to the src folder created in the local GitHub repo. And lastly choosing the framework to be .NET 6.0, setting the project to not use authentication (will be added later). Also leaving the checkbox to configure HTTPS on. Clicking the create button to create the new project.

## Optional: Github Actions

Disclaimer: I have no actual knowledge of GitHub actions, So this is just playing around.

In the GitHub Web interface I added a build action, using the predefined workflow for ".NET" and modifying the workflow to use the latest windows version and using .NET version 6.0. This should result in a build being triggered an each push to the main branch.

As this was playing around, I forgot to specify the working directory in the workflow yml file. This is needed as the visual studio solution exists in the src subfolder of the repo and not at the top level. So this should be specified. As reference, this is the content od the dotnet.yml file containing the workflow:

```
name: .NET

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build:

    runs-on: windows-latest

    steps:
    - uses: actions/checkout@v2
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: 6.0.x
    - name: Restore dependencies
      run: dotnet restore
      working-directory: src
    - name: Build
      run: dotnet build --no-restore
      working-directory: src
```

So now we are all set to start coding!! We'll start coding in the next post.
