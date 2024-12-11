using System.Text.Json;
using BlackOcean.Simulation.ShipSystems;
using static NUnit.Framework.Assert;

namespace BlackOcean.Common.Test;

public class DiffTests
{
    [Test]
    public void TypeScriptWorks()
    {
        var typescript = ModelUtil.ToTypeScript<ControlPanel>();
        Console.WriteLine(typescript);
        Pass();
    }
    
    [Test]
    public void DiffNullWorks()
    {
        var panel = new ControlPanel();

        panel.AblativeShield.Name = "Test 1";
        panel.AblativeShield.HeatBands = Band.Build(Status.Danger, (0, Status.Safe), (1, Status.Warn));
        panel.AblativeShield.LevelStatuses = [Status.Danger, Status.Safe, Status.Warn, Status.Danger];
        panel.Draw.Unit = "kelvin";
        panel.Cooler.Name = "Cooler 1";
        panel.Cooler.Pressed = true;
        
        var diff = ModelUtil.Diff(panel, null);
        That(diff.Count, Is.EqualTo(1));
        
        var diffPanel = (ControlPanel)diff[""];
        That(diffPanel.AblativeShield.Name, Is.EqualTo("Test 1"));
        That(diffPanel.Cooler.Name, Is.EqualTo("Cooler 1"));
    }
    
    
    [Test]
    public void CloneWorks()
    {
        var panel = new ControlPanel();

        panel.AblativeShield.Name = "Test 1";
        panel.AblativeShield.HeatBands = Band.Build(Status.Danger, (0, Status.Safe), (1, Status.Warn));
        panel.AblativeShield.LevelStatuses = [Status.Danger, Status.Safe, Status.Warn, Status.Danger];
        panel.Draw.Unit = "kelvin";
        panel.Cooler.Name = "Cooler 1";
        panel.Cooler.Pressed = true;
        
        var clonedPanel = ModelUtil.DeepClone(panel);
        var originalJson = JsonSerializer.Serialize(panel);
        var clonedJson = JsonSerializer.Serialize(clonedPanel);
        That(clonedJson, Is.EqualTo(originalJson));
    }
    
    [Test]
    public void DiffWorks()
    {
        var panel = new ControlPanel();

        panel.AblativeShield.Name = "Test 1";
        
        var clonedPanel = ModelUtil.DeepClone(panel);
        
        panel.AblativeShield.HeatBands = Band.Build(Status.Danger, (0, Status.Safe), (1, Status.Warn));
        panel.AblativeShield.LevelStatuses = [Status.Danger, Status.Safe, Status.Warn, Status.Danger];
        panel.Draw.Unit = "kelvin";
        panel.Cooler.Name = "Cooler 1";
        panel.Cooler.Pressed = true;
        
        var diff = ModelUtil.Diff(panel, clonedPanel);
        
        Multiple(() =>
        {
            That(diff.Count, Is.EqualTo(5));
            That(diff["AblativeShield.HeatBands"], Is.EqualTo(Band.Build(Status.Danger, (0, Status.Safe), (1, Status.Warn))));
            That(diff["AblativeShield.LevelStatuses"], Is.EqualTo(new[] { Status.Danger, Status.Safe, Status.Warn, Status.Danger }));
            That(diff["Draw.Unit"], Is.EqualTo("kelvin"));
            That(diff["Cooler.Name"], Is.EqualTo("Cooler 1"));
            That(diff["Cooler.Pressed"], Is.True);
        });
    }
    
    [Test]
    public void ApplyWorks()
    {
        var panel = new ControlPanel();

        panel.AblativeShield.Name = "Test 1";
        
        var clonedPanel = ModelUtil.DeepClone(panel);
        
        panel.AblativeShield.HeatBands = Band.Build(Status.Danger, (0, Status.Safe), (1, Status.Warn));
        panel.AblativeShield.LevelStatuses = [Status.Danger, Status.Safe, Status.Warn, Status.Danger];
        panel.Draw.Unit = "kelvin";
        panel.Cooler.Name = "Cooler 1";
        panel.Cooler.Pressed = true;
        
        var diff = ModelUtil.Diff(panel, clonedPanel);
        ModelUtil.Apply(clonedPanel, diff);
        
        Multiple(() =>
        {
            // Identify all 5 differences after the cloning
            That(diff.Count, Is.EqualTo(5));
            
            // Values are updated to match during the diff
            CollectionAssert.AreEquivalent(clonedPanel.AblativeShield.HeatBands, panel.AblativeShield.HeatBands);
            CollectionAssert.AreEquivalent(clonedPanel.AblativeShield.LevelStatuses, panel.AblativeShield.LevelStatuses);
            That(clonedPanel.Draw.Unit, Is.EqualTo(panel.Draw.Unit));
            That(clonedPanel.Cooler.Name, Is.EqualTo(panel.Cooler.Name));
            That(clonedPanel.Cooler.Pressed, Is.EqualTo(panel.Cooler.Pressed));
        });
    }
    
    [Test]
    public void DiffApplyWorks()
    {
        var panel = new ControlPanel();

        panel.AblativeShield.Name = "Test 1";
        
        var clonedPanel = ModelUtil.DeepClone(panel);
        
        panel.AblativeShield.HeatBands = Band.Build(Status.Danger, (0, Status.Safe), (1, Status.Warn));
        panel.AblativeShield.LevelStatuses = [Status.Danger, Status.Safe, Status.Warn, Status.Danger];
        panel.Draw.Unit = "kelvin";
        panel.Cooler.Name = "Cooler 1";
        panel.Cooler.Pressed = true;
        
        var diff = ModelUtil.DiffApply(panel, clonedPanel);
        
        Multiple(() =>
        {
            // Identify all 5 differences after the cloning
            That(diff.Count, Is.EqualTo(5));
            
            // Values are updated to match during the diff
            CollectionAssert.AreEquivalent(clonedPanel.AblativeShield.HeatBands, panel.AblativeShield.HeatBands);
            CollectionAssert.AreEquivalent(clonedPanel.AblativeShield.LevelStatuses, panel.AblativeShield.LevelStatuses);
            That(clonedPanel.Draw.Unit, Is.EqualTo(panel.Draw.Unit));
            That(clonedPanel.Cooler.Name, Is.EqualTo(panel.Cooler.Name));
            That(clonedPanel.Cooler.Pressed, Is.EqualTo(panel.Cooler.Pressed));
        });
    }
}