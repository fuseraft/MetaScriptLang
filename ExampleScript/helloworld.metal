# an example script
class Person
    method greet(name = "Scott")
        println "Hello! My name is {name}."
    end
end

class Developer < Person
    method writeCode()
        println "I am writing code."
    end
end

# instantiate new Developer instance
@p = Developer.new()
@p.greet("Scott Christopher Stauffer")
@p.writeCode()
