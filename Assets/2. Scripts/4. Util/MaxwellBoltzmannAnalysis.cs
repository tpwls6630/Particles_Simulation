using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MaxwellBoltzmannAnalysis
{

    private static double BoltzmannConstant => Constants.BoltzmannConstant;
    private static double AtomicMassUnitInKg => Constants.AtomicMassUnitInKg;

    /// <summary>
    /// 주어진 입자 속력 리스트에 대한 제곱평균제곱근 속력을 계산합니다.
    /// </summary>
    /// <param name="speeds">N개의 입자 속력 리스트 (단위: m/s)</param>
    /// <returns>제곱평균제곱근 속력 (단위: m/s)</returns>
    public static float RMSFromSpeeds(List<float> speeds)
    {
        float sumOfSquares = 0;
        foreach (float speed in speeds)
        {
            sumOfSquares += speed * speed;
        }
        float meanSquareSpeed = sumOfSquares / speeds.Count;
        return Mathf.Sqrt(meanSquareSpeed);
    }

    /// <summary>
    /// 주어진 입자 속력 리스트와 입자 질량으로부터 온도를 추론합니다.
    /// 3차원 공간을 가정합니다.
    /// </summary>
    /// <param name="speeds">N개의 입자 속력 리스트 (단위: m/s)</param>
    /// <param name="particleAtomicMass">입자 한 개의 원자량</param>
    /// <returns>추론된 온도 (단위: K)</returns>
    public static float TemperatureFromSpeeds(float rms, float particleAtomicMass)
    {
        if (rms <= 0)
        {
            return 0;
        }

        if (particleAtomicMass <= 0)
        {
            Debug.LogError("Particle mass must be positive.");
            throw new ArgumentOutOfRangeException(nameof(particleAtomicMass), "Particle mass must be positive.");
        }

        double particleMass = particleAtomicMass * AtomicMassUnitInKg;

        double meanSquareSpeed = rms * rms;

        double temperature = (particleMass * meanSquareSpeed) / (3 * BoltzmannConstant);

        return (float)temperature;
    }

    /// <summary>
    /// 주어진 총 입자 수, 온도, 입자 질량에 대해 맥스웰-볼츠만 속력 분포에 따른
    /// "속력 v에 대한 예상 입자 수 밀도 함수" (v -> N * f(v))를 반환합니다.
    /// 이 함수는 Func<double, double> 형태로, 속력(m/s)을 입력받아
    /// 해당 속력에서의 입자 수 밀도(입자 수 / (m/s))를 반환합니다.
    /// </summary>
    /// <param name="particleCount">총 입자 수 (N)</param>
    /// <param name="temperatureKelvin">절대 온도 T (K)</param>
    /// <param name="particleAtomicMass">입자 한 개의 원자량량</param>
    /// <returns>속력 v를 입력받아 N*f(v)를 반환하는 함수. 오류 시 v -> 0.0 함수 반환.</returns>
    public static Func<double, double> MB_DensityFuntion(
        int particleCount,
        float temperatureKelvin,
        float particleAtomicMass)
    {
        if (particleCount <= 0)
        {
            Debug.LogError("Particle count must be positive.");
            return v_speed => 0.0; // 유효하지 않은 입력 시 항상 0을 반환하는 함수
        }
        if (temperatureKelvin <= 0)
        {
            Debug.LogError("Temperature must be positive and in Kelvin.");
            return v_speed => 0.0;
        }
        if (particleAtomicMass <= 0)
        {
            Debug.LogError("Particle mass must be positive and in kg.");
            return v_speed => 0.0;
        }

        double N = particleCount;
        double T_k = temperatureKelvin;
        double m_kg = particleAtomicMass * AtomicMassUnitInKg;

        double a = Math.Sqrt(BoltzmannConstant * T_k / m_kg);

        return (double speed) =>
        {
            double v_squared = speed * speed;
            // 내부 확률 밀도 함수 f(v) 계산
            double probabilityDensity = Math.Sqrt(2.0 / Math.PI) * v_squared / (a * a * a) * Math.Exp(-v_squared / (2.0f * a * a));

            return N * probabilityDensity;
        };
    }

    public static double IntegrateDensity(Func<double, double> densityFunction, double start, double end, int step = 1000)
    {
        double sum = 0.0;
        double stepSize = (end - start) / step;
        for (int i = 0; i < step; i++)
        {
            double x = start + i * stepSize;
            sum += densityFunction(x) * stepSize;
        }

        return sum;
    }
}