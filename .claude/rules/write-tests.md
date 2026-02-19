# Testing Patterns

> **Scope**: fullstack
> **Applies when**: Creating unit tests (*.Tests projects, *.spec.ts files)

## Cross-References

| Testing... | Also load |
|------------|-----------|
| API controllers | `/dotnet-patterns` (for controller conventions) |
| Angular components | `/angular-patterns` (for component conventions) |

---

## MSTest API Tests

Create tests in `WADNR.API.Tests` project.

### Controller Test Template

```csharp
[TestClass]
public class EntityControllerTests
{
    private WADNRDbContext _dbContext;
    private EntityController _controller;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<WADNRDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new WADNRDbContext(options);
        _controller = new EntityController(_dbContext, Mock.Of<ILogger<EntityController>>());
    }

    [TestMethod]
    public async Task List_ReturnsAllEntities()
    {
        // Arrange
        _dbContext.Entities.Add(new Entity { Name = "Test" });
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _controller.List();

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result.Result;
        var entities = (List<EntityGridRowDto>)okResult.Value;
        Assert.AreEqual(1, entities.Count);
    }

    [TestMethod]
    public async Task GetByID_ReturnsEntity_WhenExists()
    {
        // Arrange
        var entity = new Entity { EntityID = 1, Name = "Test" };
        _dbContext.Entities.Add(entity);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _controller.GetByID(1);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
    }

    [TestMethod]
    public async Task GetByID_ReturnsNotFound_WhenNotExists()
    {
        // Act
        var result = await _controller.GetByID(999);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
    }
}
```

### Test Naming Convention

`{MethodName}_{ExpectedBehavior}_{Condition}`

Examples:
- `List_ReturnsAllEntities`
- `GetByID_ReturnsEntity_WhenExists`
- `GetByID_ReturnsNotFound_WhenNotExists`
- `Create_ReturnsCreatedEntity_WhenValid`
- `Update_ReturnsNoContent_WhenSuccessful`

---

## Jasmine Angular Tests

Component tests in `*.spec.ts` files alongside components.

### Component Test Template

```typescript
describe('EntityDetailComponent', () => {
  let component: EntityDetailComponent;
  let fixture: ComponentFixture<EntityDetailComponent>;
  let entityServiceSpy: jasmine.SpyObj<EntityService>;

  beforeEach(async () => {
    entityServiceSpy = jasmine.createSpyObj('EntityService', ['getByID']);

    await TestBed.configureTestingModule({
      imports: [EntityDetailComponent],
      providers: [
        { provide: EntityService, useValue: entityServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(EntityDetailComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load entity when entityID is set', fakeAsync(() => {
    const mockEntity = { entityID: 1, name: 'Test Entity' };
    entityServiceSpy.getByID.and.returnValue(of(mockEntity));

    component.entityID = '1';
    tick();

    component.entity$.subscribe(entity => {
      expect(entity).toEqual(mockEntity);
    });

    expect(entityServiceSpy.getByID).toHaveBeenCalledWith(1);
  }));

  it('should display entity name in template', fakeAsync(() => {
    const mockEntity = { entityID: 1, name: 'Test Entity' };
    entityServiceSpy.getByID.and.returnValue(of(mockEntity));

    component.entityID = '1';
    fixture.detectChanges();
    tick();
    fixture.detectChanges();

    const compiled = fixture.nativeElement;
    expect(compiled.querySelector('.card-title').textContent).toContain('Test Entity');
  }));
});
```

### Testing Async Observables

Use `fakeAsync` and `tick()` for testing observables:

```typescript
it('should handle async data', fakeAsync(() => {
  serviceSpy.getData.and.returnValue(of(mockData));

  component.loadData();
  tick();  // Advance virtual time

  expect(component.data).toEqual(mockData);
}));
```

### Service Mocking Pattern

```typescript
// Create spy with methods
const serviceSpy = jasmine.createSpyObj('ServiceName', ['method1', 'method2']);

// Configure return values
serviceSpy.method1.and.returnValue(of(mockData));
serviceSpy.method2.and.returnValue(throwError(() => new Error('Test error')));

// Provide in TestBed
providers: [{ provide: ServiceName, useValue: serviceSpy }]
```

---

## Running Tests

### API Tests
```powershell
dotnet test WADNR.API.Tests
```

### Angular Tests
```powershell
cd WADNR.Web
npm test           # Run tests once
npm run test:watch # Run tests in watch mode
```
