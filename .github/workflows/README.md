# GitHub Actions Workflows

This directory contains GitHub Actions workflows for automated testing and validation of .NET templates.

## Workflows

### 1. Quick Template Validation (`quick-validation.yml`)

**Purpose:** Fast validation for rapid feedback on pull requests and commits.

**Triggers:**
- Push to `main` or `develop` branches
- Pull requests targeting `main` or `develop`
- Only when files in `MinimalApi/` change

**What it does:**
1. Validates `template.json` syntax using `jq`
2. Installs the template locally
3. Creates test projects with:
   - Default parameters
   - All features enabled
4. Builds each project to verify compilation
5. Cleans up by uninstalling the template

**Duration:** ~3-5 minutes

**Use case:** Quick smoke test to catch obvious issues early.

---

### 2. Comprehensive Template Testing (`test-template.yml`)

**Purpose:** Thorough testing of all template parameter combinations across multiple operating systems.

**Triggers:**
- Push to `main` or `develop` branches
- Pull requests targeting `main` or `develop`
- Manual workflow dispatch

**What it does:**

Uses a matrix strategy to test all combinations:
- **Operating Systems:** Ubuntu Linux, Windows
- **Template Parameters:**
  - `includeAuth`: true, false
  - `requireApiKey`: true, false
- **Total Combinations:** 2 OS × 2 × 2 = 8 test configurations

For each configuration:
1. Installs the template
2. Generates a new project with specific parameters
3. Restores NuGet packages
4. Builds the project (Release configuration)
5. Runs the full test suite
6. Uploads test results as artifacts

**Special Considerations:**
- On Linux: Ensures Docker is available for TestContainers (used by PostgreSQL integration tests)
- Test results are preserved even if tests fail (using `if: always()`)

**Duration:** ~15-20 minutes

**Use case:** Pre-release validation and comprehensive quality assurance.

---

## Test Matrix Explanation

For the MinimalApi template, the following combinations are tested:

| Configuration | includeAuth | requireApiKey | Description |
|--------------|-------------|---------------|-------------|
| 1 | false | false | Minimal setup (default) |
| 2 | true | false | With JWT authentication |
| 3 | false | true | With API key middleware |
| 4 | true | true | All security features |

Each combination is tested on both Ubuntu and Windows, resulting in 8 total test runs.

---

## Artifacts

Test results are uploaded as artifacts with naming convention:
```
test-results-{os}-auth{includeAuth}-apikey{requireApiKey}
```

Examples:
- `test-results-ubuntu-latest-authtrue-apikeytrue`
- `test-results-windows-latest-authfalse-apikeyfalse`

Download artifacts from the Actions tab to view detailed test reports (TRX format).

---

## Adding New Template Parameters

When adding new boolean parameters to the template:

1. Update the matrix in `test-template.yml`:
   ```yaml
   matrix:
     os: [ubuntu-latest, windows-latest]
     include-auth: [true, false]
     require-api-key: [true, false]
     new-parameter: [true, false]  # Add here
   ```

2. Update the job name to include the new parameter:
   ```yaml
   name: Test (OS=${{ matrix.os }}, Auth=${{ matrix.include-auth }}, ApiKey=${{ matrix.require-api-key }}, NewParam=${{ matrix.new-parameter }})
   ```

3. Pass the parameter when generating the project:
   ```yaml
   dotnet new minapi --name TestProject --includeAuth ${{ matrix.include-auth }} --requireApiKey ${{ matrix.require-api-key }} --newParameter ${{ matrix.new-parameter }}
   ```

4. Update the artifact naming to include the new parameter.

**Note:** Each additional boolean parameter doubles the number of test combinations (e.g., 3 parameters = 16 combinations per OS).

---

## Local Testing

To test the workflows locally before pushing:

### Test the Quick Validation Flow

```bash
# Navigate to template directory
cd MinimalApi

# Validate template.json
cat .template.config/template.json | jq empty

# Install and test
dotnet new install .
cd /tmp
mkdir test-default && cd test-default
dotnet new minapi --name TestProject
dotnet build --configuration Release
```

### Test All Combinations

```bash
# Test each combination
for auth in true false; do
  for apikey in true false; do
    echo "Testing: auth=$auth, apikey=$apikey"
    cd /tmp
    mkdir "test-$auth-$apikey"
    cd "test-$auth-$apikey"
    dotnet new minapi --name TestProject --includeAuth $auth --requireApiKey $apikey
    dotnet build --configuration Release
    dotnet test --configuration Release
  done
done
```

---

## Troubleshooting

### Tests Fail on Windows but Pass on Linux

Common causes:
- Path separator differences (`/` vs `\`)
- Case-sensitive file system differences
- Line ending differences (LF vs CRLF)

**Solution:** Review template source for platform-specific assumptions.

### Docker Not Available on Linux Runners

If TestContainers fail to start:
- Verify the Docker setup step completed successfully
- Check if the test requires `sudo` permissions to access Docker
- Consider using `ubuntu-latest` runner which has better Docker support

### Template Installation Fails

Check:
- `template.json` syntax is valid (use `jq` to validate)
- Template `sourceName` matches the project names in the template
- No conflicting templates with the same `shortName` are installed

### Test Results Not Uploading

Ensure:
- The test logger generates TRX files: `--logger "trx;LogFileName=test-results.trx"`
- The path in `actions/upload-artifact` matches the actual output location
- The `if: always()` condition is present to upload even on failure

---

## Maintenance

### Updating .NET Version

When a new .NET version is released:

1. Update `dotnet-version` in both workflows
2. Update target framework in template `.csproj` files
3. Test locally before committing

### Updating Runner Images

GitHub periodically updates runner images:
- Monitor [GitHub Actions runner images changelog](https://github.com/actions/runner-images)
- Test workflows after major runner updates
- Pin to specific versions if needed: `ubuntu-22.04` instead of `ubuntu-latest`

---

## Best Practices

1. **Keep workflows fast:** Use caching for NuGet packages if builds become slow
2. **Fail fast:** Set `fail-fast: false` in matrix to see all failures, not just the first
3. **Preserve artifacts:** Always upload test results for debugging
4. **Use descriptive names:** Make job names include all matrix parameters for easy identification
5. **Test locally first:** Don't rely solely on CI; test template changes locally before pushing
