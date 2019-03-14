# Insql documentation

[![Build status](https://ci.appveyor.com/api/projects/status/92f8ydwwu5nile9q/branch/master?svg=true)](https://ci.appveyor.com/project/rainrcn/insql/branch/master)
![](https://img.shields.io/github/license/rainrcn/insql.svg?style=flat)
[![GitHub stars](https://img.shields.io/github/stars/rainrcn/insql.svg?style=social)](https://github.com/rainrcn/insql)
[![star](https://gitee.com/rainrcn/insql/badge/star.svg?theme=white)](https://gitee.com/rainrcn/insql)

> A lightweight .NET ORM framework

## 1. Introduction

**Insql is a lightweight .NET ORM framework. Object mapping is based on Dapper, and the Sql configuration is inspired by Mybatis.**

**🚀 Pursuit of simplicity, elegance, performance and quality**

Insql advocates accessing the database by writing native SQL. The overall function is divided into three major blocks:

- Unified management of SQL statements, using XML as the carrier of SQL statements, externally and uniformly managed SQL statements that are hard-coded in the program. Provides the ability to load SQL statements from a variety of sources and across multiple database SQL.
- Provides a rich mapping mechanism, using the Attribute method, the Fluent method, and the XML configuration method to implement mapping of database tables to object attributes.
- Flexible dependency injection and the use of domain-driven patterns to better manage database connections and the lifecycle of database contexts.

## 2. Installation

| Package                                                              | Nuget Stable                                                                                                                            | Downloads                                                                                                                                |
| -------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------- |
| [Insql](https://www.nuget.org/packages/Insql/)                       | [![Insql](https://img.shields.io/nuget/v/Insql.svg?style=flat)](https://www.nuget.org/packages/Insql/)                                  | [![Insql](https://img.shields.io/nuget/dt/Insql.svg?style=flat)](https://www.nuget.org/packages/Insql/)                                  |
| [Insql.MySql](https://www.nuget.org/packages/Insql.MySql/)           | [![Insql.MySql](https://img.shields.io/nuget/v/Insql.MySql.svg?style=flat)](https://www.nuget.org/packages/Insql.MySql/)                | [![Insql.MySql](https://img.shields.io/nuget/dt/Insql.MySql.svg?style=flat)](https://www.nuget.org/packages/Insql.MySql/)                |
| [Insql.Oracle](https://www.nuget.org/packages/Insql.Oracle/)         | [![Insql.Oracle](https://img.shields.io/nuget/v/Insql.Oracle.svg?style=flat)](https://www.nuget.org/packages/Insql.Oracle/)             | [![Insql.Oracle](https://img.shields.io/nuget/dt/Insql.Oracle.svg?style=flat)](https://www.nuget.org/packages/Insql.Oracle/)             |
| [Insql.PostgreSql](https://www.nuget.org/packages/Insql.PostgreSql/) | [![Insql.PostgreSql](https://img.shields.io/nuget/v/Insql.PostgreSql.svg?style=flat)](https://www.nuget.org/packages/Insql.PostgreSql/) | [![Insql.PostgreSql](https://img.shields.io/nuget/dt/Insql.PostgreSql.svg?style=flat)](https://www.nuget.org/packages/Insql.PostgreSql/) |
| [Insql.Sqlite](https://www.nuget.org/packages/Insql.Sqlite/)         | [![Insql.Sqlite](https://img.shields.io/nuget/v/Insql.Sqlite.svg?style=flat)](https://www.nuget.org/packages/Insql.Sqlite/)             | [![Insql.Sqlite](https://img.shields.io/nuget/dt/Insql.Sqlite.svg?style=flat)](https://www.nuget.org/packages/Insql.Sqlite/)             |

## 3. Features

- **Support DotNet Core 2.0+ & DotNet Framework 4.6.1+**
- **Support for dependency injection systems**
- **Similar to MyBatis sql xml configuration syntax**
- **Multiple database support**
- **high performance**
- **Flexible scalability**
- **Simple and intuitive to use**

## 4. Use

### 4.1 Using Insql

```csharp
public void ConfigureServices(IServiceCollection services)
{
  services.AddInsql();  //Will use the default configuration
}
```

### 4.2 Setting up Insql

When we use it normally, we can use the default configuration, and you don't need to set the following options. Detailed parameters will be explained in other sections

```csharp
public void ConfigureServices(IServiceCollection services)
{
  services.AddInsql(builder =>
  {
      //Add an embedded assembly SQL XML configuration file
      builder.AddEmbeddedXml();

      //Add a SQL XML configuration file in the external file directory to specify the directory address
      builder.AddExternalXml();

      //Add SQL parsing filter for logging
      builder.AddResolveFilter();

      //Add a SQL parsing description provider that can be extended to load SQL XML configuration files from multiple sources, such as loading SQL XML configuration from a database. EmbeddedXml and ExternalXml are the extensions
      builder.AddDescriptorProvider();

      //Set default dynamic script parser parameters
      builder.AddDefaultScriptResolver();

      //Set default multiple database matcher parameters
      builder.AddDefaultResolveMatcher();
  });
}
```

### 4.3 Sample Code

#### 4.3.1 Basic usage examples

`UserDbContext.insql.xml`

```xml
<insql type="Insql.Tests.Domain.Contexts.UserDbContext,Insql.Tests" >

  <!--Define UserInfo type database fields to object attribute mappings-->
  <map type="Insql.Tests.Domain.Models.UserInfo,Insql.Tests">
    <key name="user_id" to="UserId" />
    <column name="user_name" to="UserName" />
    <column name="user_gender" to="UserGender" />
  </map>

  <select id="GetUser">
    select * from user_info where user_id = @userId
  </select>

</insql>
```

**_Note: In the default setting, the User.insql.xml file requires the right-click property to select the `embedded assembly file` method to take effect._**

`UserDbContext.cs`

```csharp
//`type` in insql.xml needs to correspond to `UserDbContext` type
public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    public UserInfo GetUser(int userId)
    {
        //The sqlId parameter is "GetUser" corresponding to the sql id in insql.xml
        //sqlParam parameter supports PlainObject and IDictionary<string,object> types
        return this.Query<UserInfo>(nameof(GetUser), new { userId }).SingleOrDefault();
    }
}
```

Use UserDbContext in `ValuesController.cs` or `Domain Service` for presentation convenience in the Controller.

```csharp
[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    private readonly UserDbContext dbContext;

    public UserService(UserDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet]
    public ActionResult<IEnumerable<string>> Get()
    {
        //select User
        var userInfo = this.dbContext.GetUser("tome");

        //Can also be used like this, directly call sql through dbContext, and write the same method in DbContext
        var roleList = this.dbContext.Query<RoleInfo>("GetRoleList");

        return new string[] { "value1", "value2" };
    }
}
```

`Startup.cs` Register UserDbContext

```csharp
public void ConfigureServices(IServiceCollection services)
{
    //Register Insql
    services.AddInsql();

    //Register UserDbContext
    services.AddInsqlDbContext<UserDbContext>(options =>
    {
      //Select UserDbContext database connection
      //options.UseSqlServer(this.Configuration.GetConnectionString("sqlserver"));
      options.UseSqlite(this.Configuration.GetConnectionString("sqlite"));
    });
}
```

This is the complete use process, the example is to use the domain-driven model, you can use the situation depending on the situation. For example, UserDbContext can be injected into the Controller without the UserService.

#### 4.3.2 Use transaction

```csharp
public void InsertUserList(IEnumerable<UserInfo> infoList)
{
    try
    {
        this.dbContext.BeginTransaction();

        foreach (var item in infoList)
        {
            this.dbContext.InsertUserSelective(item);
        }

        this.dbContext.CommitTransaction();
    }
    catch
    {
        this.dbContext.RollbackTransaction();

        throw;
    }
}
```

Use the `DoWithTransaction` extension method to automatically start and commit the transaction, and automatically roll back when an exception is encountered. If it is currently in a transaction, this extension will not start and commit the transaction again.

```csharp
public void InsertUserList(IEnumerable<UserInfo> infoList)
{
    this.dbContext.DoWithTransaction(() =>
    {
        foreach (var item in infoList)
        {
            this.dbContext.InsertUserSelective(item);
        }
    });
}
```

Use the `DoWithOpen` extension method to automatically open the connection and close the connection. If the current connection has already been opened, the connection will not be opened and closed again.

```csharp
public void InsertUserList(IEnumerable<UserInfo> infoList)
{
    this.dbContext.DoWithOpen(() =>
    {
        foreach (var item in infoList)
        {
            this.dbContext.InsertUserSelective(item);
        }
    });
}
```

#### 4.3.3 SELECT IN

For the use of SELECT IN arrays, there are two

```csharp
  var sqlParam = new { userIdList = new string[]{ "tom","jerry" } };
```

**1. Use each**

```xml
<select id="EachIn">
  select * from user_info
  where user_id in
  <each name="userIdList" open="(" separator="," close=")" prefix="@"  />
</select>
```

After the SqlResolver is parsed, the SQL statement becomes as follows:

```sql
select * from user_info where user_id in (@userIdList1,@userIdList2)
```

_If you only use ISqlResolver.Resolve to parse SQL statements, it is recommended to use this method. If you use it normally, it is more convenient to use the following._

**2. Use the list parameter conversion function supported by Dapper**

```xml
<select id="DapperIn">
  select * from user_info
  where user_id in @userIdList
</select>
```

When Dapper executes, it will change the SQL statement as follows, and then execute:

```sql
select * from user_info where user_id in (@userIdList1,@userIdList2)
```

## 5. Configuration syntax

**xxx.insql.xml** configuration syntax is similar to the configuration syntax Mybatis currently supports the following configuration section :

- **map**
  - **key**
  - **column**
- **sql**
  - **include**
  - **bind**
  - **if**
  - **where**
  - **set**
  - **trim**
  - **each**
- **select** = **sql**
- **insert** = **sql**
- **update** = **sql**
- **delete** = **sql**

### 5.1 map

`map` the configuration section is used for mapping of database table fields to object properties so that they `DbContext.Query<UserInfo>`vwill be used as long as they are queried

```xml
<map type="Insql.Tests.Domain.Models.UserInfo,Insql.Tests">
  <key name="user_id" to="UserId" />
  <column name="user_name" to="UserName" />
  <column name="user_gender" to="UserGender" />
</map>
```

| Child element name | Attribute name | Property description | Description                      |
| ------------------ | -------------- | -------------------- | -------------------------------- |
| `key`              |                |                      | Indicates the primary key column |
|                    | `name*`        | Column name          |                                  |
|                    | `to*`          | Object property name |                                  |
| `column`           |                |                      | Represents a normal column       |
|                    | `name*`        | Column name          |                                  |
|                    | `to*`          | Object property name |                                  |

### 5.2 sql

`sql` the configuration section is used to configure database execution statements.`select`,`insert`,`update`,`delete` with `sql` an `sql`alias that has the same functionality, just a configuration section.

```xml
<sql id="userColumns">
  user_id as UserId,user_name as UserName,user_gender as UserGender
</sql>

<select id="GetUser">
  select
  <include refid="userColumns" />
  from user_info
  where user_id = @userId
</select>
```

| Child element name | Attribute name    | Property description                                                                                                     | Description                                                                                                                                                                                                                                 |
| ------------------ | ----------------- | ------------------------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `include`          |                   |                                                                                                                          | Import other `sql` configuration sections                                                                                                                                                                                                   |
|                    | `refid*`          | Configuration section to import `id`                                                                                     |                                                                                                                                                                                                                                             |
| `bind`             |                   |                                                                                                                          | Create a new query parameter to the current parameter list, such as like fuzzy query scene                                                                                                                                                  |
|                    | `name*`           | New parameter name created                                                                                               |                                                                                                                                                                                                                                             |
|                    | `value*`          | Dynamic script expression, for example: '%'+userName+'%'                                                                 |                                                                                                                                                                                                                                             |
|                    | `valueType`       | Specifies `value` the type returned, in the format System.TypeCode enumeration,value types are best specified explicitly |                                                                                                                                                                                                                                             |
| `if`               |                   |                                                                                                                          | Determine the dynamic expression, if it is satisfied, output the internal content                                                                                                                                                           |
|                    | `test*`           | Dynamic expression, you need to return a bool type, for example: userName != null                                        |                                                                                                                                                                                                                                             |
| `where`            |                   |                                                                                                                          | Add the `where` sql segment at the current position , whether the output `where` depends on whether its internal child elements have valid content output, and will overwrite the beginning `and`,`or`                                      |
| `set`              |                   |                                                                                                                          | Add the `set` sql segment at the current position , mainly used in the `update` configuration section, whether the output `set` depends on whether its internal child elements have valid content output, and will overwrite the ending `,` |
| `trim`             |                   |                                                                                                                          | Trimming the output of the wrapped element, wrapping the child elements with the specified prefix character and suffix character                                                                                                            |
|                    | `prefix`          | Package prefix character                                                                                                 |                                                                                                                                                                                                                                             |
|                    | `suffix`          | Package suffix character                                                                                                 |                                                                                                                                                                                                                                             |
|                    | `prefixOverrides` | Will overwrite the specified character at the beginning of the internal output                                           |                                                                                                                                                                                                                                             |
|                    | `suffixOverrides` | Will override the specified character at the end of the internal output                                                  |                                                                                                                                                                                                                                             |
| `each`             |                   |                                                                                                                          | Loop array type of query parameter for each value                                                                                                                                                                                           |
|                    | `name*`           | Loop array parameter name                                                                                                |                                                                                                                                                                                                                                             |
|                    | `separator`       | Separator between each value                                                                                             |                                                                                                                                                                                                                                             |
|                    | `open`            | The left side of the package                                                                                             |                                                                                                                                                                                                                                             |
|                    | `close`           | The right side of the package                                                                                            |                                                                                                                                                                                                                                             |
|                    | `prefix`          | Each value name prefix                                                                                                   |                                                                                                                                                                                                                                             |
|                    | `suffix`          | Suffix for each value name                                                                                               |                                                                                                                                                                                                                                             |

`include`,`where`,`if`,`bind`

```xml
<select id="GetUserList">
    <include refid="selectUserColumns" />
    <where>
        <if test="userName != null">
          <bind name="likeUserName" value="'%' + userName + '%'" />
          user_name like @likeUserName
        </if>
        and user_gender = @userGender
    </where>
    order by  user_id
  </select>
```

`set`

```xml
<update id="UpdateUserSelective">
  update user_info
  <set>
    <if test="UserName != null">
      user_name=@UserName,
    </if>
    user_gender=@UserGender
  </set>
  where user_id = @UserId
</update>
```

`trim`

```xml
<insert id="InsertUserSelective">
    insert into user
    <trim prefix="(" suffix=")" suffixOverrides=",">
      user_id,
      <if test="UserName != null">
        user_name,
      </if>
      <if test="UserGender != null">
        user_gender,
      </if>
      create_time,
      <if test="LastLoginTime != null">
        last_login_time,
      </if>
    </trim>
    <trim prefix="values (" suffix=")" suffixOverrides=",">
      @UserId,
      <if test="UserName != null">
        @UserName,
      </if>
      <if test="UserGender != null">
        @UserGender,
      </if>
      @CreateTime,
      <if test="LastLoginTime != null">
        @LastLoginTime,
      </if>
    </trim>
  </insert>
```

`each`

```xml
<select id="EachIn">
  select * from user_info
  where user_id in
  <each name="userIdList" open="(" separator="," close=")" prefix="@"  />
</select>
```

After SqlResolver parsing :

```sql
select * from user_info where user_id in (@userIdList1,@userIdList2)
```

**_Note: After parsing will delete the original `userIdList`parameters, and increase `userIdList1`,`userIdList2` the parameters_**

_Tip: Dapper's own parameter list conversion function can also be used on select in list._

## 6. Dynamic script

The dynamic script syntax is JAVASCRIPT. Support for common object properties of ECMAScript 6.

```xml
<if test="userGender !=null and userGender == 'W' ">
  and user_gender = @userGender
</if>
```

`userGender !=null and userGender == 'W'` Part of it is a dynamic script.

### 6.1 operator conversion

Because `&`, `<` these have special meaning in XML, so support for these symbols in the dynamic conversion script. The following symbol conversions are currently supported:

| Before conversion | After conversion |
| ----------------- | ---------------- |
| `and`             | `&&`             |
| `or`              | `\|\|`           |
| `gt`              | `>`              |
| `gte`             | `>=`             |
| `lt`              | `<`              |
| `lte`             | `<=`             |
| `eq`              | `==`             |
| `neq`             | `!=`             |

_The operator conversion function can be disabled or the conversion of some of the operators can be excluded._

**_Note: Please avoid the same query parameters as the above operator name. If it is unavoidable, you can set to exclude conflicting operators. Then implement the operator with the xml transfer symbol_**

### 6.2 Enumeration converted to a string

`userGender == 'W'` `userGender` the attribute is an enumerated type, which is converted to a character-for-format by default in dynamic scripts. This conversion can be disabled, and the enum will be converted to a `number` type after disabling .

### 6.3 Time type conversion

If the query parameter contains a time type `DateTime`, it will be converted to the `Date` type in JS, because the minimum time of Date is 1970.1.1, so if there is an unassigned DateTime (0001.1.1) in the query object, or less than 1970 The DateTime of the time will be converted to 1970.1.1 by default, and the conversion only happens when the dynamic script is run, and does not affect the original value of the query parameters. If there is an unassigned `DateTime?` type in the parameter object, it will be null itself and will not be converted.

### 6.4 Setting up dynamic scripts

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddInsql(builder =>
    {
        builder.AddDefaultScriptResolver(options =>
        {
            options.IsConvertOperator = false;  //Disable operator conversion
            options.IsConvertEnum = false; //Disable enum conversion to string
            options.ExcludeOperators = new string[]
            {
                "eq","neq"  //Exclude eq, neq operator conversion
            };
        });
    });
}
```

## 7. Multiple database matching

```xml
<!--By default, the example uses the Sqlite database-->
<insert id="InsertUser">
  insert into user_info (user_name,user_gender) values (@UserName,@UserGender);
  select last_insert_rowid() from user_info;
</insert>
<!--MySql-->
<insert id="InsertUser.MySql">
  insert into user_info (user_name,user_gender) values (@UserName,@UserGender);
  select LAST_INSERT_ID();
</insert>
<!--SqlServer-->
<insert id="InsertUser.SqlServer">
  insert into user_info (user_name,user_gender) values (@UserName,@UserGender);
  select SCOPE_IDENTITY();
</insert>
```

### 7.1 Set up multiple database matching

```csharp
public void ConfigureServices(IServiceCollection services)
{
  services.AddInsql(builder=>
  {
      builder.AddDefaultResolveMatcher(options=>
      {
          options.CorssDbEnabled = false; //Whether to enable multi-database matching, enabled by default
          options.CorssDbSeparator = "@"; //Multi-database match separator, default is `.`
      });
  });
}
```

_The match separator will change to the following:_

```xml
<insert id="InsertUser">
  insert into user_info (user_name,user_gender) values (@UserName,@UserGender);
  select LAST_INSERT_ID();
</insert>
<!--SqlServer-->
<insert id="InsertUser@SqlServer">
  insert into user_info (user_name,user_gender) values (@UserName,@UserGender);
  select SCOPE_IDENTITY();
</insert>
```

### 7.2 Matching rule

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddInsqlDbContext<UserDbContext>(options =>
    {
      //Which SqlId to match, which database to use
      options.UseSqlServer(this.Configuration.GetConnectionString("sqlserver"));
      //options.UseSqlite(this.Configuration.GetConnectionString("sqlite"));
    });

    services.AddScoped<IUserService,UserService>();
}
```

**_If you are currently using SqlServer, the suffix with `.SqlServer` will be matched first. Matches the default statement without a suffix if it is not found._**

**_Currently supports matching database suffixes: `SqlServer` `Sqlite` `MySql` `Oracle` `PostgreSql`_**

### 7.3 Extended database support

There is no limit to the support of other databases. As long as the database to be supported has a client library that supports .NET, it is very easy to support. Just implement the `IDbSessionFactory` interface.

## 8. Multiple configuration sources

### 8.1 Embedding assembly file mode source

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddInsql(builder =>
    {
        builder.AddEmbeddedXml(options =>
        {
            options.Enabled = false;    //This source feature can be disabled and is enabled by default.
            //options.Matches = "**/*.insql.xml"; //Glob file filter expression, this is the default value
            //...
        });
    });
}
```

### 8.2 External file directory mode source

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddInsql(builder =>
    {
        builder.AddExternalXml(options=>
        {
            options.Enabled = true; //This source can be started, the default is disabled
            options.Directory = "D:\\Insqls";   //Configure the load directory, support recursive search, subfolders will also scan, the default is the current program execution directory
            //options.Matches = "**/*.insql.xml"; //Glob file filter expression, this is the default value
        });
    });
}
```

### 8.3 Multi-configuration source merge function

`EmbeddedXml` and the `ExternalXml` mode can be enabled at the same time. For the same file with insql type, the latter will overwrite the same statement configuration with the former sqlId and the same mapping configuration with map type.

## 9. Extended function

### 9.1 Statement Parsing Filter

Create a statement-resolved logging filter

```csharp
public class LogResolveFilter : ISqlResolveFilter
{
  private readonly ILogger<LogResolveFilter> logger;

  public LogResolveFilter(ILogger<LogResolveFilter> logger)
  {
      this.logger = logger;
  }

  public void OnResolved(ResolveContext resolveContext, ResolveResult resolveResult)
  {
      this.logger.LogInformation($"insql resolved id : {resolveContext.InsqlSection.Id} , sql : {resolveResult.Sql}");
  }

  public void OnResolving(InsqlDescriptor insqlDescriptor, string dbType, string sqlId, IDictionary<string, object> sqlParam)
  {
  }
}
```

`OnResolving` execute before `OnResolved` parsing, execute after parsing

**Enable filters:**

```csharp
public void ConfigureServices(IServiceCollection services)
{
  services.AddInsql(builder =>
  {
      builder.AddResolveFilter<LogResolveFilter>();
  });
}
```

### 9.2 Statement Configuration Description Provider

```csharp
public interface IInsqlDescriptorProvider
{
    IEnumerable<InsqlDescriptor> GetDescriptors();
}
```

The implementation of the above interface can be achieved, the specific implementation details can refer to `EmbeddedXml` or `ExternalXml`part of the source code. The detailed implementation details will be written in the future.

## 10. Tools

### 10.1 Code Generator

The `tools` CodeSmith generator file is included in the source directory, and you can run these files directly after installing CodeSmith.

![code_generator](code_generator.zh_cn.png)

**Generate code example: show only one data table**

`UserPo.cs`

```csharp
namespace Tests.Domain.Model
{
	public class UserPo
	{
    /// <summary>
    /// user_id
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// user_name
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// user_gender
    /// </summary>
    public string UserGender { get; set; }

    /// <summary>
    /// user_intro
    /// </summary>
    public string UserIntro { get; set; }

    /// <summary>
    /// create_time
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// last_login_time
    /// </summary>
    public DateTime? LastLoginTime { get; set; }
	}
}
```

`TestDbContext.cs`

```csharp
namespace Tests.Domain.Context
{
  public class UserDbContext : DbContext
  {
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    /*
        UserPo
    */
    public int SelectUserCountByKey(int UserId)
    {
        return this.Query<UserPo>(nameof(SelectUserCountByKey),new
        {
          UserId
        }).Count();
    }

    public UserPo SelectUserByKey(int UserId)
    {
        return this.Query<UserPo>(nameof(SelectUserByKey),new
        {
          UserId
        }).SingleOrDefault();
    }

    public void InsertUser(UserPo info)
    {
        this.Execute(nameof(InsertUser),info);
    }

    public void InsertUserSelective(UserPo info)
    {
        this.Execute(nameof(InsertUserSelective),info);
    }

    public void UpdateUserByKey(UserPo info)
    {
        this.Execute(nameof(UpdateUserByKey),info);
    }

    public void UpdateUserSelectiveByKey(UserPo info)
    {
        this.Execute(nameof(UpdateUserSelectiveByKey),info);
    }
    public void DeleteUserByKey(int UserId)
    {
        this.Execute(nameof(DeleteUserByKey),new
        {
          UserId
        });
    }
  }
}
```

`TestDbContext.insql.xml`

```xml
<insql type="Tests.Domain.Context.UserDbContext,Tests.Domain">
  <!--
    user
  -->
  <map type="Tests.Domain.Model.UserPo,Tests.Domain">
   <key name="user_id" to="UserId" />
   <column name="user_name" to="UserName" />
   <column name="user_gender" to="UserGender" />
   <column name="user_intro" to="UserIntro" />
   <column name="create_time" to="CreateTime" />
   <column name="last_login_time" to="LastLoginTime" />
  </map>

  <select id="SelectUserCountByKey">
    select count(*) from `user` where `user_id` = @UserId
  </select>

  <select id="SelectUserByKey">
    select * from `user` where `user_id` = @UserId
  </select>

  <insert id="InsertUser">
    insert into `user` (`user_id`,`user_name`,`user_gender`,`user_intro`,`create_time`,`last_login_time`) values (@UserId,@UserName,@UserGender,@UserIntro,@CreateTime,@LastLoginTime)
  </insert>

  <insert id="InsertUserSelective">
    insert into `user`
    <trim prefix="(" suffix=")" suffixOverrides=",">
      `user_id`,
      `user_name`,
      `user_gender`,
      <if test="UserIntro != null">
        `user_intro`,
      </if>
      `create_time`,
      <if test="LastLoginTime != null">
        `last_login_time`,
      </if>
    </trim>
    <trim prefix="values (" suffix=")" suffixOverrides=",">
      @UserId,
      @UserName,
      @UserGender,
      <if test="UserIntro != null">
        @UserIntro,
      </if>
      @CreateTime,
      <if test="LastLoginTime != null">
        @LastLoginTime,
      </if>
    </trim>
  </insert>

  <update id="UpdateUserByKey">
    update `user`
    <set>
     `user_name` = @UserName,
     `user_gender` = @UserGender,
     `user_intro` = @UserIntro,
     `create_time` = @CreateTime,
     `last_login_time` = @LastLoginTime,
    </set>
    where `user_id` = @UserId
  </update>

  <update id="UpdateUserSelectiveByKey">
    update `user`
    <set>
      <if test="UserName != null">
        `user_name` = @UserName,
      </if>
      <if test="UserGender != null">
        `user_gender` = @UserGender,
      </if>
      <if test="UserIntro != null">
        `user_intro` = @UserIntro,
      </if>
      `create_time` = @CreateTime,
      <if test="LastLoginTime != null">
        `last_login_time` = @LastLoginTime,
      </if>
    </set>
    where `user_id` = @UserId
  </update>
  <delete id="DeleteUserByKey">
    delete from `user` where `user_id` = @UserId
  </delete>
</insql>
```

## 11. performance

To ask about the performance, there is no need to say more, OK will be done. :) just kidding. Because of the Dapper used for object mapping, there is no need to worry about performance. Basically, it is consistent with Dapper and has little fluctuation. A performance test may be written later.

## 12. Update

- 1.8.2

  - Rewrite and beautify the documentation
  - Optimize dynamic script execution engine to reduce resource allocation and improve performance
  - Optimize the code generator to solve some bugs in generated code

- 1.5.0
  - Supports map configuration blocks for mapping database table fields to class attribute fields. Make mapping when querying objects easier, without the need for an alias.
  - Supports SQL configuration file directory source, can load SQL configuration from specified file directory, and supports merge with embedded SQL configuration
  - Optimize dynamic script parsing for conversion of DateTime.Min

## 13. Planning

- Support feature Attribute and Fluent mode database table and object mapping mode
- Increase the CURD simple extension of the object
