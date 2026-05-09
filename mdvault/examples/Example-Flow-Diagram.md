# Example Flow Diagram

This diagram shows what happens when `TomcatUserValidator` runs from the command line.

<a href="/assets/example-flow-diagram.svg" target="_blank" rel="noopener" title="Open the example flow diagram">
  <img src="/assets/example-flow-diagram.svg" alt="Example flow diagram">
</a>

## Example Inputs

- Username/password arguments are passed after `--` in `dotnet run`.
- The sample XML file is available at <a href="/examples/tomcat/tomcat-users.xml">examples/tomcat/tomcat-users.xml</a>.
- Logging configuration is copied from `conf/log4net.config` into the example output.
