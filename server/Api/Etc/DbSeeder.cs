using DataAccess;
using DataAccess.Entities;

namespace Api.Etc;

public class DbSeeder(
    AppDbContext context
//, IPasswordHasher<User> hasher
)
{
    public async Task SetupAsync()
    {
        context.Database.EnsureCreated();
        if (!context.Users.Any())
        {
            await CreateUsers(
                [
                    (email: "admin@example.com", role: Role.Admin),
                    (email: "editor@example.com", role: Role.Editor),
                    (email: "othereditor@example.com", role: Role.Editor),
                    (email: "reader@example.com", role: Role.Reader),
                ]
            );
            await context.SaveChangesAsync();
        }

        if (!context.Posts.Any(p => p.PublishedAt != null))
        {
            var admin = context.Users.Single((user) => user.Email == "admin@example.com");
            context.Posts.AddRange(
                new Post
                {
                    Title = "First post",
                    Content =
                        @"## Hello Python
Have you ever wondered how to make a hello-world application in Python?

The answer is simply:
```py
print('Hello World!')
```
                    ",
                    AuthorId = admin!.Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = DateTime.UtcNow,
                    PublishedAt = DateTime.UtcNow.AddDays(-1),
                },
                new Post
                {
                    Title = "First post",
                    Content =
                @"
# Linux (CLI)

## Files and folders

The file system key in Linux.

```bash
# Change directory
cd Documents

# Navigate to parent folder
cd .

# Go to home (your users) folder
cd ~

# Print current working directory
pwd

# List files
ls

# List files in a specific directory
ls Documents

# List files with details (long)
ls -l

# List hidden files also
ls -a
# NOTE: hidden files start with a ""."" (dot)

# Make an empty file
touch file.txt
# NOTE: actually ""touch"" is meant to update the last accessed/modified timestamp, but it will also create the file if it doesn't exist.

# Append some text to a file
echo ""Hello World"" >> file.txt

# Print content of a file
cat file1.txt

# Print concatenated content of several files
cat file1.txt file2.txt

# Scroll through a file
less file.txt

# Remove a file
rm file.txt

# Remove a directory and all of its contents (no trash/recycle bin)
rm - r Documents
```
                ",

                    AuthorId = admin!.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    PublishedAt = DateTime.UtcNow,
                },
                new Post
                {
                    Title = "Docker Intro",
                    Content =
                        @"# Docker CLI Introduction

## Commands

This section contains a list of useful docker commands.
It is only for reference.
You don't have to type them (yet).

Parameters in angle brackets are replaced by user input.
Example: `<image>`

### Shell

Run a shell from a container image.

```bash
docker run -it --rm <image> sh
```

### Show containers

Show running containers.

```bash
docker ps
```

Show all containers, even those that are not actively running.

```bash
docker ps
```

### Shell in running container

```bash
docker exec -it <id or name> sh
```

You can find ID and Name with `docker ps` command.

### Show logs

```bash
docker logs <id or name>
```

### Build an image

```bash
docker build <directory>
```

Where `<directory>` is a folder containing a `Dockerfile`.

It will create with a long HEX code as name.

You can name and version an image by tagging it.

```bash
docker build --tag <name>:<version> <directory>
```
                    ",
                    AuthorId = admin!.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    PublishedAt = DateTime.UtcNow,
                }
            );
        }
        if (!context.Posts.Any(p => p.PublishedAt == null))
        {
            var editor = context.Users.Single((user) => user.Email == "editor@example.com");
            context.Posts.Add(
                new Post
                {
                    Title = "Draft",
                    Content = "This is a draft post",
                    AuthorId = editor!.Id,
                    CreatedAt = DateTime.UtcNow,
                    PublishedAt = null,
                }
            );
        }
        await context.SaveChangesAsync();

        if (!context.Comments.Any())
        {
            var reader = context.Users.Single((user) => user.Email == "reader@example.com");
            context.Comments.Add(
                new Comment
                {
                    Content = "First one to comment",
                    AuthorId = reader.Id,
                    PostId = context.Posts.First().Id,
                }
            );
            await context.SaveChangesAsync();
        }
    }

    private async Task CreateUsers((string email, string role)[] users)
    {
        foreach (var user in users)
        {
            context.Users.Add(
                new User
                {
                    UserName = user.email.Split("@")[0],
                    Email = user.email,
                    EmailConfirmed = true,
                    Role = user.role,
                    // PasswordHash = hasher.HashPassword(null, defaultPassword),
                }
            );
        }
        await context.SaveChangesAsync();
    }
}
