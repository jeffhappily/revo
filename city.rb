require 'csv'

class City
    @@distances = Hash.new { |hash, key| hash[key] = {} }

    def self.init
        CSV.foreach('input.csv', headers: true) do |csv| 
            csv.headers.each do |city|
                next if city.nil?

                @@distances[csv[0]][city] = csv[city].to_i
            end
        end
    end

    def self.distances
        @@distances
    end

    def self.cities
        distances.keys
    end
end